using NUnit.Framework;
using System.Reflection;
using Detrack.ElasticRoute;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Tests
{
    public class DepotValidationTest
    {
        private static Random rng = new Random();

        public Depot CreateDepot(string testName = null, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            string depotName = testName ?? memberName + System.DateTime.Now.Ticks;
            string[] testAddresses =
            { "61 Kaki Bukit Ave 1 #04-34, Shun Li Ind Park Singapore 417943",
            "8 Somapah Road Singapore 487372",
            "80 Airport Boulevard (S)819642",
            "80 Mandai Lake Road Singapore 729826",
            "10 Bayfront Avenue Singapore 018956",
            "18 Marina Gardens Drive Singapore 018953"
            };
            string depotAddress = testAddresses[rng.Next(testAddresses.Length)];
            Depot depot = new Depot(depotName, depotAddress);
            return depot;
        }
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestMustHaveAtLeastOneDepot()
        {
            List<Depot> depots = new List<Depot>();
            try
            {
                Depot.ValidateDepots(depots);
                Assert.Fail("No exception was thrown");
            }
            catch (BadFieldException ex)
            {
                Assert.AreEqual("You must have at least one depot", ex.Message);
            }
        }

        [Test]
        public void TestNamesMustBeDistinct()
        {
            List<Depot> depots = new List<Depot>{
                this.CreateDepot(),
                this.CreateDepot("bad"),
                this.CreateDepot("bad")
            };
            try
            {
                Depot.ValidateDepots(depots);
                Assert.Fail("No exception was thrown");
            }
            catch (BadFieldException ex)
            {
                Assert.AreEqual("Depot name must be distinct", ex.Message);
            }
            depots = new List<Depot>{
                this.CreateDepot(),
                this.CreateDepot()
            };
            Assert.True(Depot.ValidateDepots(depots));
        }

        [Test]
        public void TestNamesCannotBeNull([Values(null, "", " ")]string testName)
        {
            try
            {
                Depot depot = new Depot(null, "8 Somapah Road");
                Assert.Fail("No exception was thrown");
            }
            catch (BadFieldException ex)
            {
                Assert.AreEqual("Depot name cannot be null", ex.Message);
            }
        }

        [Test]
        public void TestNamesCannotBeLongerThan255Chars()
        {
            string longName = new string('A', 256);
            try
            {
                Depot depot = new Depot(longName, "8 Somapah Road");
                Assert.Fail("No exception was thrown");
            }
            catch (BadFieldException ex)
            {
                Assert.AreEqual("Depot name cannot be more than 255 chars", ex.Message);
            }
        }

        [Test]
        public void TestCanPassCoordinatesOnly()
        {
            List<Depot> depots = new List<Depot>
            {
                this.CreateDepot(),
                this.CreateDepot()
            };
            depots[1].Address = null;
            depots[1].Lat = 1.3368888888888888f;
            depots[1].Lng = 103.91086111111112f;
            Assert.IsTrue(Depot.ValidateDepots(depots));
        }

        [Test]
        public void TestCannotPassNoFormsOfAddress()
        {
            List<Depot> depots = new List<Depot>
            {
                this.CreateDepot()
            };
            depots[0].Address = null;
            try
            {
                Depot.ValidateDepots(depots);
                Assert.Fail("No exception was thrown");
            }catch(BadFieldException ex){
                Assert.AreEqual("Depot address and coordinates are not given", ex.Message);
            }
        }
    }
}