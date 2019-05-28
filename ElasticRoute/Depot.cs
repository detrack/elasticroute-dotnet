using System;
using System.Reflection;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Detrack.ElasticRoute
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Depot
    {
        private string _name;
        [JsonProperty("name")]
        public string Name
        {
            get { return _name; }
            set
            {
                if (value == null || value.Trim() == "")
                {
                    throw new BadFieldException("Depot name cannot be null");
                }
                else if (value.Length > 255)
                {
                    throw new BadFieldException("Depot name cannot be more than 255 chars");
                }
                else
                {
                    _name = value;
                }
            }
        }
        [JsonProperty("address")]
        public string Address { get; set; }
        [JsonProperty("postal_code")]
        public string PostalCode { get; set; }
        [JsonProperty("lat")]
        public float? Lat { get; set; }
        [JsonProperty("lng")]
        public float? Lng { get; set; }
        [JsonProperty("default")]
        public bool? Default { get; set;}

        [JsonConstructor]
        internal Depot(string name)
        {
            this.Name = name;
        }

        public Depot(string name, string address)
        {
            this.Name = name;
            this.Address = address;
        }

        public Depot(string name, float lat, float lng)
        {
            this.Name = name;
            this.Lat = lat;
            this.Lng = lng;
        }

        public static bool ValidateDepots(List<Depot> depots)
        {
            if (depots.Count < 1)
            {
                throw new BadFieldException("You must have at least one depot");
            }
            foreach (Depot depot in depots)
            {
                bool sameNameDepot(Depot v) => v.Name == depot.Name;
                if (depots.FindAll((Predicate<Depot>)sameNameDepot).Count > 1)
                {
                    throw new BadFieldException("Depot name must be distinct");
                }
                if (depot.Lat == null || depot.Lng == null)
                {
                    if (depot.Address == null)
                    {
                        throw new BadFieldException("Depot address and coordinates are not given");
                    }
                }
            }
            return true;
        }

        public Depot Clone()
        {
            string json = JsonConvert.SerializeObject(this);
            return JsonConvert.DeserializeObject<Depot>(json);
        }

        public void Absorb(Depot other)
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
