using System;
using System.Reflection;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Detrack.ElasticRoute
{
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy), ItemNullValueHandling = NullValueHandling.Ignore)]
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
                if (string.IsNullOrWhiteSpace(value))
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
        public int? From {get; set;}
        public int? Till {get; set;}
        public string AssignTo {get; internal set;}
        public int Run { get; internal set; }
        public int Sequence {get; internal set;}
        public string Eta {get; internal set;}
        public string Exception {get; internal set;}

        [JsonConstructor]
        internal Stop(string name)
        {
            this.Name = name;
        }

        public Stop(string name, string address)
        {
            this.Name = name;
            this.Address = address;
        }

        public Stop(String name, float lat, float lng)
        {
            this.Name = name;
            this.Lat = lat;
            this.Lng = lng;
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

        public Stop Clone()
        {
            string json = JsonConvert.SerializeObject(this);
            return JsonConvert.DeserializeObject<Stop>(json);
        }

        public void Absorb(Stop other)
        {
            PropertyInfo[] properties = other.GetType().GetProperties();
            foreach(PropertyInfo property in properties)
            {
                PropertyInfo internalProperty = this.GetType().GetProperty(property.Name);
                object value = property.GetValue(other);
                if(value != null)
                {
                    Type internalType = internalProperty.PropertyType;
                    Type internalUnderlyingType = Nullable.GetUnderlyingType(internalType);
                    internalProperty.SetValue(this, Convert.ChangeType(value, internalUnderlyingType ?? internalType), null);
                }
                else if(value == null)
                {
                    internalProperty.SetValue(this, null, null);
                }
            }
        }
    }
}
