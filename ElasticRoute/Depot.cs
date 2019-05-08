using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Detrack.ElasticRoute
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Depot
    {
#pragma warning disable CS0169, IDE0051 // Remove unused private members
        private string _name;
#pragma warning restore CS0169, IDE0051 // Remove unused private members

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

        public Depot()
        {
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

    }
}
