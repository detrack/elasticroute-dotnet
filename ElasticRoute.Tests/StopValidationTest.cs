using NUnit.Framework;
using System.Reflection;
using Detrack.ElasticRoute;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Tests
{
    public class StopValidationTest
    {
        private static Random rng = new Random();

        public Stop CreateStop(string testName = null, [System.Runtime.CompilerServices.CallerMemberName]
            string memberName = "")
        {
            string stopName = testName ?? memberName + System.DateTime.Now.Ticks;
            string[] testAddresses =
            {
                "61 Kaki Bukit Ave 1 #04-34, Shun Li Ind Park Singapore 417943",
                "8 Somapah Road Singapore 487372",
                "80 Airport Boulevard (S)819642",
                "80 Mandai Lake Road Singapore 729826",
                "10 Bayfront Avenue Singapore 018956",
                "18 Marina Gardens Drive Singapore 018953"
            };
            string stopAddress = testAddresses[rng.Next(testAddresses.Length)];
            Stop stop = new Stop(stopName, stopAddress);
            return stop;
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestMustHaveAtLeastTwoStops()
        {
            List<Stop> stops = new List<Stop>();
            var ex = Assert.Throws<BadFieldException>(() => Stop.ValidateStops(stops));
            Assert.AreEqual("You must have at least two stops", ex.Message);
        }

        [Test]
        public void TestNamesMustBeDistinct()
        {
            List<Stop> stops = new List<Stop>
            {
                this.CreateStop(),
                this.CreateStop("bad"),
                this.CreateStop("bad")
            };
            var ex = Assert.Throws<BadFieldException>(() => Stop.ValidateStops(stops));
            Assert.AreEqual("Stop name must be distinct", ex.Message);
            stops = new List<Stop>
            {
                this.CreateStop(),
                this.CreateStop()
            };
            Assert.True(Stop.ValidateStops(stops));
        }

        [Test]
        public void TestNamesCannotBeNull([Values(null, "", " ")] string testName)
        {
            var ex = Assert.Throws<BadFieldException>(() => new Stop("", ""));
            Assert.AreEqual("Stop name cannot be null", ex.Message);
        }

        [Test]
        public void TestNamesCannotBeLongerThan255Chars()
        {
            string longName = new string('A', 256);
            var ex = Assert.Throws<BadFieldException>(() => new Stop(longName, ""));
            Assert.AreEqual("Stop name cannot be more than 255 chars", ex.Message);
        }

        [Test]
        public void TestCanPassCoordinatesOnly()
        {
            List<Stop> stops = new List<Stop>
            {
                this.CreateStop(),
                this.CreateStop()
            };
            stops[1].Address = null;
            stops[1].Lat = 1.3368888888888888f;
            stops[1].Lng = 103.91086111111112f;
            Assert.IsTrue(Stop.ValidateStops(stops));
        }

        [Test]
        public void TestCannotPassNoFormsOfAddress()
        {
            List<Stop> stops = new List<Stop>
            {
                this.CreateStop(),
                this.CreateStop()
            };
            stops[0].Address = null;
            var ex = Assert.Throws<BadFieldException>(() => Stop.ValidateStops(stops));
            Assert.AreEqual("Stop address and coordinates are not given", ex.Message);
        }

        [Test]
        public void TestPositiveNumericFields([Values("WeightLoad", "VolumeLoad", "SeatingLoad")]
            string field)
        {
            float value = -100f;
            List<Stop> stops = new List<Stop>
            {
                this.CreateStop(),
                this.CreateStop(),
            };
            var ex = Assert.Throws<TargetInvocationException>(() => stops[1].GetType().GetProperty(field).SetValue(stops[1], value));
            Assert.IsInstanceOf(typeof(BadFieldException), ex.InnerException);
            Assert.AreEqual($"Stop {field} cannot be negative", ex.InnerException.Message);
        }
    }
}