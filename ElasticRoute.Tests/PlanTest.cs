using NUnit.Framework;
using System.Reflection;
using Detrack.ElasticRoute;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace Tests
{
    [TestFixture]
    public class PlanTest
    {
        [OneTimeSetUp]
        public void SetUpClass()
        {
            Console.WriteLine(System.IO.Directory.GetCurrentDirectory());
            DotNetEnv.Env.Load("../../../.env");
            Plan.BaseURI = new Uri(Environment.GetEnvironmentVariable("elasticroute_path"));
            Plan.DefaultApiKey = Environment.GetEnvironmentVariable("elasticroute_api_key");
            if (DotNetEnv.Env.GetBool("elasticroute_proxy_enabled"))
            {
                Plan.handler.UseProxy = true;
                Plan.handler.Proxy = new System.Net.WebProxy(Environment.GetEnvironmentVariable("elasticroute_proxy_path"));
                Plan.handler.ServerCertificateCustomValidationCallback = (a,b,c,d) => true;
            }
        }


        [Test]
        public void TestConstructor()
        {
            Plan plan = new Plan();
            plan.Id = "TestPlan_1234567890";
            Depot mainWarehouse = new Depot();
            mainWarehouse.Name = "Main Warehouse";
            mainWarehouse.Address = "8 Somapah Road";
            Vehicle mainVehicle = new Vehicle();
            mainVehicle.Name = "Van 1";
            Stop stop1 = new Stop();
            stop1.Name = "Customer 1";
            stop1.Address = "15 Simei Street 4";
            Stop stop2 = new Stop();
            stop2.Name = "Customer 2";
            stop2.Address = "59 Changi South Avenue 4";
            plan.Depots.Add(mainWarehouse);
            plan.Vehicles.Add(mainVehicle);
            plan.Stops.AddRange(new List<Stop>{stop1, stop2});
            Console.WriteLine(plan.Solve().Result);
        }
    }
}
