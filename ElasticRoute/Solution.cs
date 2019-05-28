using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
namespace Detrack.ElasticRoute
{
    public class Solution
    {
        public static string DefaultApiKey {get; set;}
        public string ApiKey {get; set;}
        public string Id {get; set;}
        [JsonProperty(PropertyName = "stops")]
        public List<Stop> Stops {get; set; } = new List<Stop>();
        [JsonProperty(PropertyName = "vehicles")]
        public List<Vehicle> Vehicles { get; set;} = new List <Vehicle>();
        [JsonProperty(PropertyName = "depots")]
        public List<Depot> Depots { get; set;} = new List<Depot>();
        [JsonConverter(typeof(StringEnumConverter))]
        public Plan.ConnectionTypes ConnectionType;

        public Plan.GeneralSettingsTemplate GeneralSettings = new Plan.GeneralSettingsTemplate();
        public static HttpClientHandler handler = new HttpClientHandler();
        public static readonly HttpClient client = new HttpClient(handler);
        public Solution(Plan plan)
        {
            this.ApiKey = plan.ApiKey;
            this.Id = plan.Id;
            foreach(Stop stop in plan.Stops)
            {
                this.Stops.Add(stop.Clone());
            }
            foreach(Vehicle vehicle in plan.Vehicles)
            {
                this.Vehicles.Add(vehicle.Clone());
            }
            foreach(Depot depot in plan.Depots)
            {
                this.Depots.Add(depot.Clone());
            }
            this.ConnectionType = plan.ConnectionType;
            this.GeneralSettings = plan.GeneralSettings;
        }
    }
}
