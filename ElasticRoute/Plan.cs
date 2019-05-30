using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

[assembly: CLSCompliant(true)]
namespace Detrack.ElasticRoute
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class Plan
    {
        public static Uri BaseURI {get; set;}
        public static string DefaultApiKey {get; set;}
        public string ApiKey {get; set;}
        public string Id {get; set;}
        public DateTime SubmittedAt {get; internal set;}
        [JsonProperty(PropertyName = "stops")]
        public List<Stop> Stops {get; set; } = new List<Stop>();
        [JsonProperty(PropertyName = "vehicles")]
        public List<Vehicle> Vehicles { get; set;} = new List <Vehicle>();
        [JsonProperty(PropertyName = "depots")]
        public List<Depot> Depots { get; set;} = new List<Depot>();
        public enum ConnectionTypes { sync, poll, webhook }
        [JsonConverter(typeof(StringEnumConverter))]
        public ConnectionTypes ConnectionType;


        [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy), ItemNullValueHandling = NullValueHandling.Ignore)]
        public class GeneralSettingsTemplate
        {
            public string Country = "SG";
            public string Timezone = "Asia/Singapore";
            public int? LoadingTime;
            public int? Buffer;
            public int? ServiceTime;
            public enum DistanceUnits { km, miles };
            [JsonConverter(typeof(StringEnumConverter))]
            public DistanceUnits DistanceUnit {get;set;} = DistanceUnits.km;
            public int? MaxTime;
            public int? MaxDistance;
            public int? MaxStops;
            public int? MaxRuns;
            private int? _avail_from;
            public int? AvailFrom{
                get { return _avail_from; }
                set
                {
                    if(value > 2400)
                    {
                        throw new BadFieldException("AvailFrom of GeneralSettings must be between 0 and 2400");
                    }
                    else
                    {
                        _avail_from = value;
                    }
                }
            }
            private int? _avail_till;
            public int? AvailTill{
                get { return _avail_till; }
                set
                {
                    if(value > 2500)
                    {
                        throw new BadFieldException("AvailTill of GeneralSettings must be between 0 and 2400");
                    }
                    else
                    {
                        _avail_till = value;
                    }
                }
            }
            public Uri WebhookURL;
        }
        [JsonProperty(PropertyName = "generalSettings")]
        public GeneralSettingsTemplate GeneralSettings = new GeneralSettingsTemplate();

        public string Status {get; internal set; }
        public int Progress { get; internal set; }

        public static HttpClientHandler handler = new HttpClientHandler();
        public static readonly HttpClient client = new HttpClient(handler);


        static Plan()
        {
            BaseURI = new Uri("https://app.elasticroute.com/api/v1/plan");
            handler.AllowAutoRedirect = true;
            handler.UseCookies = false;

        }
        public Plan(string id)
        {
            this.Id = id;
        }

        public async Task Solve()
        {
            if(string.IsNullOrWhiteSpace(this.Id)){
                throw new BadFieldException("You neeed to create an id for this plan!");
            }
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {this.ApiKey ?? DefaultApiKey}");
            client.BaseAddress = BaseURI;
            client.Timeout = System.Threading.Timeout.InfiniteTimeSpan;
            string requestPath = $"plan/{this.Id}";
            if(this.ConnectionType == ConnectionTypes.sync)
            {
                requestPath += "?c=sync";
            }else if(this.ConnectionType == ConnectionTypes.webhook)
            {
                requestPath += "?w=true";
            }
            //validate data
            Stop.ValidateStops(this.Stops);
            Depot.ValidateDepots(this.Depots);
            Vehicle.ValidateVehicles(this.Vehicles);

            //fill in defaults
            foreach(Stop stop in this.Stops)
            {
                if (!stop.From.HasValue)
                {
                    stop.From = this.GeneralSettings.AvailFrom;
                }
                if (!stop.Till.HasValue)
                {
                    stop.Till = this.GeneralSettings.AvailTill;
                }
                if (string.IsNullOrWhiteSpace(stop.Depot))
                {
                    stop.Depot = this.Depots[0].Name;
                }
            }
            foreach(Vehicle vehicle in this.Vehicles)
            {
                if (!vehicle.AvailFrom.HasValue)
                {
                    vehicle.AvailFrom = this.GeneralSettings.AvailFrom;
                }
                if (!vehicle.AvailTill.HasValue)
                {
                    vehicle.AvailTill = this.GeneralSettings.AvailTill;
                }
                if (string.IsNullOrWhiteSpace(vehicle.Depot))
                {
                    vehicle.Depot = this.Depots[0].Name;
                }
            }
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ContractResolver = new Detrack.ElasticRoute.Tools.CustomContractResolver();
            settings.Formatting = Formatting.Indented;
            
            StringContent content = new StringContent(JsonConvert.SerializeObject(this, settings));
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var responseTask = client.PostAsync(requestPath, content);
            HttpResponseMessage responseMessage = await responseTask;
            var stringTask = responseMessage.Content.ReadAsStringAsync();
            string responseString = await stringTask;
            if (!responseMessage.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"API Return Code {(int) responseMessage.StatusCode}");
            }
            JObject responseObject = JObject.Parse(responseString);
            foreach(JToken stopToken in responseObject["data"]["details"]["stops"])
            {
                Stop receivedStop = stopToken.ToObject<Stop>();
                Stop internalStop = this.Stops.Find(x => x.Name == receivedStop.Name);
                internalStop.Absorb(receivedStop);
            }
            foreach(JToken vehicleToken in responseObject["data"]["details"]["vehicles"])
            {
                Vehicle receivedVehicle = vehicleToken.ToObject<Vehicle>();
                Vehicle internalVehicle = this.Vehicles.Find(x => x.Name == receivedVehicle.Name);
                internalVehicle.Absorb(receivedVehicle); 
            }
            foreach(JToken depotToken in responseObject["data"]["details"]["depots"])
            {
                Depot receivedDepot = depotToken.ToObject<Depot>();
                Depot internalDepot = this.Depots.Find(x => x.Name == receivedDepot.Name);
                internalDepot.Absorb(receivedDepot);
                
            }
            this.Progress = responseObject["data"]["progress"].Value<int>();
            this.Status = responseObject["data"]["stage"].Value<string>();
            this.Id = responseObject["data"]["plan_id"].Value<string>();
            this.SubmittedAt = responseObject["data"]["submitted"].Value<DateTime>();
        }
    }
}
