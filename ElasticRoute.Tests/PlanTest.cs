﻿using NUnit.Framework;
using Detrack.ElasticRoute;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
            Plan.DefaultBaseURI = new Uri(Environment.GetEnvironmentVariable("elasticroute_path"));
            Plan.DefaultApiKey = Environment.GetEnvironmentVariable("elasticroute_api_key");
            if (DotNetEnv.Env.GetBool("elasticroute_proxy_enabled"))
            {
                Plan.handler.UseProxy = true;
                Plan.handler.Proxy = new System.Net.WebProxy(Environment.GetEnvironmentVariable("elasticroute_proxy_path"));
                Plan.handler.ServerCertificateCustomValidationCallback = (a, b, c, d) => true;
            }
        }


        [Test]
        public async Task SimplePlanTest()
        {
            Plan plan = new Plan("TestPlan_1234567890");
            Depot mainWarehouse = new Depot("Main Warehouse", "8 Somapah Road");
            Vehicle mainVehicle = new Vehicle("Van 1");
            Stop stop1 = new Stop("Customer 1", "15 Simei Street 4");
            Stop stop2 = new Stop("Customer 2", "59 Changi South Avenue 1");
            plan.Depots.Add(mainWarehouse);
            plan.Vehicles.Add(mainVehicle);
            plan.Stops.AddRange(new List<Stop> { stop1, stop2 });
            await plan.Solve();
            Assert.AreEqual("planned", plan.Status);
            Assert.AreEqual(100, plan.Progress);
            Console.WriteLine(plan.Stops[0].Name);
            Console.WriteLine(plan.Stops[0].Eta);
            Assert.AreEqual("Van 1", plan.Stops[0].AssignTo);
            Assert.AreEqual(1, plan.Stops[0].Run);
            Assert.AreEqual(2, plan.Stops[0].Sequence);
        }

        [Test]
        public async Task TestPoll()
        {
            string fileData = File.ReadAllText("../../../bigData.json");
            JObject testData = JObject.Parse(fileData);
            Plan testPlan = new Plan("TestPlan2_" + ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds());
            testPlan.ConnectionType = Plan.ConnectionTypes.poll;
            testPlan.Stops = testData["stops"].ToObject<List<Stop>>();
            testPlan.Vehicles = testData["vehicles"].ToObject<List<Vehicle>>();
            testPlan.Depots = testData["depots"].ToObject<List<Depot>>();
            testPlan.Solve().Wait();
            Assert.AreEqual("submitted", testPlan.Status);
            while (testPlan.Status != "planned")
            {
                await testPlan.Refresh();
                await Task.Delay(1000);
            }
            Assert.IsNotEmpty(testPlan.Stops.Where(x => !String.IsNullOrEmpty(x.AssignTo)));
        }
    }
}
