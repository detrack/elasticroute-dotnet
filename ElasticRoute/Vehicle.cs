using System;
using System.Collections.Generic;

namespace Detrack.ElasticRoute
{
    public class Vehicle
    {
        private string depot;
        private string name;
        private int priority = 1;
        private float weight_capacity;
        private float volume_capacity;
        private float seating_capacity;
        private int buffer;
        private string avail_from;
        private string avail_till;
        private bool return_to_depot = false;
        private List<string> vehicle_types;
        public string Name
        {
            get { return name; }
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
                    name = value;
                }
            }
        }
        public int Priority { get; set; }
        public float WeightCapacity
        {
            get { return weight_capacity; }
            set
            {
                if (value < 0)
                {
                    throw new BadFieldException("Vehicle weight_capacity cannot be null");
                }
                else
                {
                    weight_capacity = value;
                }
            }
        }
        public float VolumeCapacity
        {
            get { return volume_capacity; }
            set
            {
                if (value < 0)
                {
                    throw new BadFieldException("Vehicle volume_capacity cannot be null");
                }
                else
                {
                    volume_capacity = value;
                }
            }
        }
        public float SeatingCapacity
        {
            get { return seating_capacity; }
            set
            {
                if (value < 0)
                {
                    throw new BadFieldException("Vehicle seating_capacity cannot be null");
                }
                else
                {
                    seating_capacity = value;
                }
            }
        }
        public int Buffer { get; set; }
        public string AvailFrom { get; set; }
        public string AvailTill { get; set; }
        public string ReturnToDepot { get; set; }
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
