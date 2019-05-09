using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Detrack.ElasticRoute
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class Plan
    {
        public static Uri BaseURI {get; set;}
        public static string DefaultApiKey {get; set;}
        public string ApiKey {get; set;}
        public string Id {get; set;}
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
            public uint? LoadingTime;
            public uint? Buffer;
            public uint? ServiceTime;
            public enum DistanceUnits { km, miles };
            [JsonConverter(typeof(StringEnumConverter))]
            public DistanceUnits DistanceUnit {get;set;} = DistanceUnits.km;
            public uint? MaxTime;
            public uint? MaxDistance;
            public uint? MaxStops;
            public uint? MaxRuns;
            private uint? _avail_from;
            public uint? AvailFrom{
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
            private uint? _avail_till;
            public uint? AvailTill{
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
        public static HttpClientHandler handler = new HttpClientHandler();
        public static readonly HttpClient client = new HttpClient(handler);

        static Plan()
        {
            BaseURI = new Uri("https://app.elasticroute.com/api/v1/plan");
            handler.AllowAutoRedirect = true;
            handler.UseCookies = false;

        }
        public Plan()
        {
        }

        public async Task<string> Solve()
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

            StringContent content = new StringContent(JsonConvert.SerializeObject(this));
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var responseTask = client.PostAsync(requestPath, content);
            HttpResponseMessage responseMessage = await responseTask;
            Console.WriteLine((int) responseMessage.StatusCode);
            var stringTask = responseMessage.Content.ReadAsStringAsync();
            string responseString = await stringTask;
            return responseString;
        }
    }
}
