using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Detrack.ElasticRoute
{
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy), ItemNullValueHandling = NullValueHandling.Ignore)]
    public class Vehicle
    {
        private string _name;
        private float? _weight_capacity;
        private float? _volume_capacity;
        private float? _seating_capacity;
        public string Depot { get; set;}
        public string Name
        {
            get { return _name; }
            set
            {
                if (value == null || value.Trim() == "")
                {
                    throw new BadFieldException("Vehicle name cannot be null");
                }else if(value.Length > 255)
                {
                    throw new BadFieldException("Vehicle name cannot be more than 255 chars");
                }
                else
                {
                    _name = value;
                }
            }
        }
        public int Priority { get; set; }
        public float? WeightCapacity
        {
            get { return _weight_capacity; }
            set
            {
                if (value.HasValue && value < 0)
                {
                    throw new BadFieldException("Vehicle WeightCapacity cannot be negative");
                }
                else
                {
                    _weight_capacity = value;
                }
            }
        }
        public float? VolumeCapacity
        {
            get { return _volume_capacity; }
            set
            {
                if (value.HasValue && value < 0)
                {
                    throw new BadFieldException("Vehicle VolumeCapacity cannot be negative");
                }
                else
                {
                    _volume_capacity = value;
                }
            }
        }
        public float? SeatingCapacity
        {
            get { return _seating_capacity; }
            set
            {
                if (value.HasValue && value < 0)
                {
                    throw new BadFieldException("Vehicle SeatingCapacity cannot be negative");
                }
                else
                {
                    _seating_capacity = value;
                }
            }
        }
        public int Buffer { get; set; }
        public uint? AvailFrom { get; set; }
        public uint? AvailTill { get; set; }
        public bool? ReturnToDepot { get; set; }
        public List<string> VehicleTypes { get; set; }


        public static bool ValidateVehicles(List<Vehicle> vehicles)
        {
            if(vehicles.Count == 0)
            {
                throw new BadFieldException("You must have at least one vehicle");
            }
            foreach(Vehicle vehicle in vehicles)
            {
                Predicate<Vehicle> sameNameVehicle = (Vehicle v) => { return v.Name == vehicle.Name; };
                if (vehicles.FindAll(sameNameVehicle).Count > 1)
                {
                    throw new BadFieldException("Vehicle name must be distinct");
                }
            }
            return true;
        }
    }
}
