using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Detrack.ElasticRoute
{
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class Stop
    {
        private string _name;
        private float? _weight_load;
        private float? _volume_load;
        private float? _seating_load;
        public string VehicleType {get; set;}
        public string Depot {get; set;}
        public string Group {get; set;}
        public string Name
        {
            get { return _name; }
            set
            {
                if (value == null || value.Trim() == "")
                {
                    throw new BadFieldException("Stop name cannot be null");
                }
                else if (value.Length > 255)
                {
                    throw new BadFieldException("Stop name cannot be more than 255 chars");
                }
                else
                {
                    _name = value;
                }
            }
        }
        public string TimeWindow {get; set;}
        public string Address {get; set;}
        public string PostalCode {get; set;}
        public float? WeightLoad
        {
            get { return _weight_load; }
            set
            {
                if (value.HasValue && value < 0)
                {
                    throw new BadFieldException("Stop WeightLoad cannot be negative");
                }
                else
                {
                    _weight_load = value;
                }
            }
        }
        public float? VolumeLoad
        {
            get { return _volume_load; }
            set
            {
                if (value.HasValue && value < 0)
                {
                    throw new BadFieldException("Stop VolumeLoad cannot be negative");
                }
                else
                {
                    _volume_load = value;
                }
            }
        }
        public float? SeatingLoad
        {
            get { return _seating_load; }
            set
            {
                if (value.HasValue && value < 0)
                {
                    throw new BadFieldException("Stop SeatingLoad cannot be negative");
                }
                else
                {
                    _seating_load = value;
                }
            }
        }
        public float? ServiceTime {get; set;}
        public float? Lat {get; set;}
        public float? Lng {get; set;}
        public string From {get; set;}
        public string Till {get; set;}
        public string AssignTo {get; set;}
        public string Run {get; set;}
        public string Sequence {get; set;}
        public string Eta {get; set;}
        public string Exception {get; set;}

        public Stop()
        {
        }

        public static bool ValidateStops(List<Stop> stops)
        {
            if (stops.Count < 2)
            {
                throw new BadFieldException("You must have at least two stops");
            }
            foreach (Stop stop in stops)
            {
                bool sameNameStop(Stop v) => v.Name == stop.Name;
                if (stops.FindAll((Predicate<Stop>)sameNameStop).Count > 1)
                {
                    throw new BadFieldException("Stop name must be distinct");
                }
                if (stop.Lat == null || stop.Lng == null)
                {
                    if (stop.Address == null)
                    {
                        throw new BadFieldException("Stop address and coordinates are not given");
                    }
                }
            }
            return true;
        }
    }
}
