using NUnit.Framework;
using System.Reflection;
using Detrack.ElasticRoute;
using System.Collections.Generic;
namespace Tests
{
    public class VehicleValidationTest
    {
        public Vehicle CreateVehicle(string testName = null, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            Vehicle vehicle = new Vehicle();
            vehicle.Name = testName ?? memberName + System.DateTime.Now.Ticks;
            return vehicle;
        }
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestMustHaveAtLeastOneVehicle()
        {
            List<Vehicle> vehicles = new List<Vehicle>();
            try
            {
                Vehicle.ValidateVehicles(vehicles);
                Assert.Fail("No exception was thrown");
            }catch (BadFieldException ex)
            {
                Assert.AreEqual("You must have at least one vehicle", ex.Message);
            }
        }

        [Test]
        public void TestNamesMustBeDistinct()
        {
            List<Vehicle> vehicles = new List<Vehicle>{
                this.CreateVehicle(),
                this.CreateVehicle("bad"),
                this.CreateVehicle("bad")
            };
            try
            {
                Vehicle.ValidateVehicles(vehicles);
                Assert.Fail("No exception was thrown");
            }
            catch (BadFieldException ex)
            {
                Assert.AreEqual("Vehicle name must be distinct", ex.Message);
            }
            vehicles = new List<Vehicle>{
                this.CreateVehicle(),
                this.CreateVehicle()
            };
            Assert.True(Vehicle.ValidateVehicles(vehicles));
        }

        [Test]
        public void TestNamesCannotBeNull([Values(null,""," ")]string testName)
        {
            Vehicle vehicle = new Vehicle();
            try { 
                vehicle.Name = null;
                Assert.Fail("No exception was thrown");
            }catch(BadFieldException ex)
            {
                Assert.AreEqual("Vehicle name cannot be null", ex.Message);
            }
        }

        [Test]
        public void TestNamesCannotBeLongerThan255Chars()
        {
            string longName = new string('A', 256);
            Vehicle vehicle = new Vehicle();
            try
            {
                vehicle.Name = longName;
                Assert.Fail("No exception was thrown");
            }catch(BadFieldException ex)
            {
                Assert.AreEqual("Vehicle name cannot be more than 255 chars", ex.Message);
            }
        }

        [Test]
        public void TestPositiveNumericFields([Values("WeightCapacity", "VolumeCapacity", "SeatingCapacity")] string field)
        {
            float value = -100f; 
            List<Vehicle> vehicles = new List<Vehicle>
            {
                this.CreateVehicle(),
                this.CreateVehicle(),
            };
            try
            {
                vehicles[1].GetType().GetProperty(field).SetValue(vehicles[1], value);
                Assert.Fail("No exception was thrown");
            }
            catch (TargetInvocationException ex)
            {
                Assert.IsInstanceOf(typeof(BadFieldException), ex.InnerException);
                Assert.AreEqual($"Vehicle {field} cannot be negative", ex.InnerException.Message);
            }
        }
    }
}