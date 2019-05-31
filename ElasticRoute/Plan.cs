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
    /// <summary>
    /// Represents a route plan used to route your fleet. Contains <see cref="Stop"/>s, <see cref="Vehicles"/>s and <see cref="Depots"/>.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class Plan
    {
        /// <summary>
        /// Gets or sets the base URI at which the ElasticRoute API is found. You should only change this if you are self-hosting the ElasticRoute server.
        /// </summary>
        /// <value>The base URI.</value>
        public static Uri BaseURI { get; set; }
        /// <summary>
        /// Gets or sets the default API key used to authenticate against the API. Your API Key can be found in the dashboard.
        /// </summary>
        /// <value>The default API key.</value>
        public static string DefaultApiKey { get; set; }
        /// <summary>
        /// Gets or sets the API key for just this <see cref="Plan"/> instance, overrides <see cref="Plan.DefaultApiKey"/>.
        /// </summary>
        /// <value>The API key.</value>
        public string ApiKey { get; set; }
        /// <summary>
        /// Gets or sets the unique id of this plan. Required.
        /// </summary>
        /// <value>The identifier.</value>
        public string Id { get; set; }
        /// <summary>
        /// Gets the <see cref="DateTime"/> this plan was originally submitted at
        /// </summary>
        /// <value>The submitted at DateTime</value>
        public DateTime SubmittedAt { get; internal set; }
        /// <summary>
        /// Gets or sets the list of <see cref="Stop"/> in this plan.
        /// </summary>
        /// <value>The stops.</value>
        [JsonProperty(PropertyName = "stops")]
        public List<Stop> Stops { get; set; } = new List<Stop>();
        /// <summary>
        /// Gets or sets the list of <see cref="Vehicle"/>s in this plan.
        /// </summary>
        /// <value>The vehicles.</value>
        [JsonProperty(PropertyName = "vehicles")]
        public List<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        /// <summary>
        /// Gets or sets the list of <see cref="Depot"/>s in this plan.
        /// </summary>
        /// <value>The depots.</value>
        [JsonProperty(PropertyName = "depots")]
        public List<Depot> Depots { get; set; } = new List<Depot>();
        /// <summary>
        /// The different connection types that can be used for a plan
        /// </summary>
        public enum ConnectionTypes
        {
            /// <summary>
            /// Synchronous connection mode. The server will solve the plan completely before returning a response. Max processing time 30 seconds.
            /// </summary>
            sync,
            /// <summary>
            /// Poll mode. The server will return a response immediately before solving the plan. You will then need to call <see cref="Plan.Refresh"/> periodically to get updates.
            /// </summary>
            poll,
            /// <summary>
            /// Webhook mode. Similar to poll mode, but server will also send a copy of the response to the specfied webhook. Specify the webhook URL using the <see cref="Plan.GeneralSettingsTemplate.WebhookURL"/> field of <see cref="Plan.GeneralSettings"/>.
            /// </summary>
            webhook
        }
        /// <summary>
        /// Specifies the connection type for this plan. See <see cref="Plan.ConnectionTypes"/> for the list of possible types.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public ConnectionTypes ConnectionType;

        /// <summary>
        /// Template for the <see cref="Plan.GeneralSettings"/> field. Used to control plan solving parameters.
        /// </summary>
        [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy), ItemNullValueHandling = NullValueHandling.Ignore)]
        public class GeneralSettingsTemplate
        {
            /// <summary>
            /// The country used to geocode the addresses of your <see cref="Stop"/>s and <see cref="Depot"/>s. String must correspond to a <a href="https://en.wikipedia.org/wiki/List_of_ISO_3166_country_codes">ISO 3166-1 alpha-2</a> country code.
            /// </summary>
            public string Country { get; set; } = "SG";
            /// <summary>
            /// The timezone to represent your ETA's in. String must correspond to a canonical <a href="https://en.wikipedia.org/wiki/List_of_tz_database_time_zones">tz database timezone</a>.
            /// </summary>
            public string Timezone { get; set; } = "Asia/Singapore";
            /// <summary>
            /// Specifies additional time to load goods/products into vehicle to factor in during calculation
            /// </summary>
            public int? LoadingTime;
            /// <summary>
            /// Additional buffer time in percentage points. If specified, additional buffer time will be added to each stop calculation
            /// </summary>
            public int? Buffer;
            /// <summary>
            /// Specifies additional service time (minutes) to be added to the stop. This value will be used if the <see cref="Stop.ServiceTime"/> is empty for a given stop
            /// </summary>
            public int? ServiceTime;
            /// <summary>
            /// The different units of measurement available to display distances
            /// </summary>
            public enum DistanceUnits
            {
                /// <summary>
                /// Displays distances in kilometres
                /// </summary>
                km,
                /// <summary>
                /// Displays distances in miles
                /// </summary>
                miles
            };
            /// <summary>
            /// Specifies the units of measurement used to display distances (default kilometres)
            /// </summary>
            [JsonConverter(typeof(StringEnumConverter))]
            public DistanceUnits DistanceUnit { get; set; } = DistanceUnits.km;
            /// <summary>
            /// Specifies the maximum allowable time for <see cref="Vehicle"/>s in their run. Leave as <code>null</code> for no limit.
            /// </summary>
            public int? MaxTime;
            /// <summary>
            /// Specifies the maximum allowable distance for the <see cref="Vehicle"/>s in their run. Leave as <code>null</code> for no limit.
            /// </summary>
            public int? MaxDistance;
            /// <summary>
            /// Specifies the maximum allowable stops for the <see cref="Vehicle"/>s to visit in their run. Leave as <code>null</code> for no limit.
            /// </summary>
            public int? MaxStops;
            /// <summary>
            /// Specifies the maximum allowable runs in a plan. Leave as <code>null</code> for no limit.
            /// </summary>
            public int? MaxRuns;
            private int? _from;
            /// <summary>
            /// Specifies the default delivery window / vehicle shift starting time for stops and vehicles. This value will be used if <see cref="Stop.From"/> and <see cref="Vehicle.AvailFrom"/> are empty.
            /// </summary>
            /// <value>An integer between 0 and 2400</value>
            public int? From
            {
                get => _from;
                set
                {
                    if (value > 2400)
                    {
                        throw new BadFieldException("AvailFrom of GeneralSettings must be between 0 and 2400");
                    }
                    else
                    {
                        _from = value;
                    }
                }
            }
            private int? _till;
            /// <summary>
            /// Specifies the default delivery window / vehicle shift ending time for stops and vehicles. This value will be used if <see cref="Stop.Till"/> and <see cref="Vehicle.AvailTill"/> are empty.
            /// </summary>
            /// <value>Am integer between 0 and 2400</value>
            public int? Till
            {
                get { return _till; }
                set
                {
                    if (value > 2400)
                    {
                        throw new BadFieldException("AvailTill of GeneralSettings must be between 0 and 2400");
                    }
                    else
                    {
                        _till = value;
                    }
                }
            }
            /// <summary>
            /// Specifies the location where a POST request will be sent to by the server after planning has been complete.
            /// </summary>
            public Uri WebhookURL;
        }
        /// <summary>
        /// Used to control plan solving parameters. See <see cref="Plan.GeneralSettingsTemplate"/> for parameters you can change.
        /// </summary>
        [JsonProperty(PropertyName = "generalSettings")]
        public GeneralSettingsTemplate GeneralSettings = new GeneralSettingsTemplate();

        /// <summary>
        /// Retrieves the status of the plan, either "planned" or "submitted".
        /// </summary>
        public string Status { get; internal set; }
        /// <summary>
        /// Retrieves the planning progress percentage point (out of 100).
        /// </summary>
        public int Progress { get; internal set; }

        /// <summary>
        /// Instance of <see cref="HttpClientHandler"/> used by <see cref="Plan.client"/>
        /// </summary>
        public static HttpClientHandler handler = new HttpClientHandler();
        /// <summary>
        /// Instance of <see cref="HttpClient"/> used by this library to make HTTP requests
        /// </summary>
        public static readonly HttpClient client = new HttpClient(handler);


        static Plan()
        {
            BaseURI = new Uri("https://app.elasticroute.com/api/v1/plan");
            handler.AllowAutoRedirect = true;
            handler.UseCookies = false;

        }
        /// <summary>
        /// Initializes a new Plan instance with the specified id. Id is required.
        /// </summary>
        /// <param name="id">Identifier (name)</param>
        public Plan(string id)
        {
            this.Id = id;
        }

        /// <summary>
        /// Sends the details of this plan to the ElasticRoute server and await the response. Depends on the <see cref="Plan.ConnectionType"/> property, this <see cref="Task"/> may complete either immediately or only when the server has finish routing all your stops and vehicles.
        /// </summary>
        /// <exception cref="BadFieldException">Thrown if <see cref="Plan.Id"/> was not yet set before calling this method</exception>
        /// <exception cref="HttpRequestException">Thrown if the API returns a HTTP status other than 200.</exception>
        public async Task Solve()
        {
            if (string.IsNullOrWhiteSpace(this.Id))
            {
                throw new BadFieldException("You neeed to create an id for this plan!");
            }
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {this.ApiKey ?? DefaultApiKey}");
            client.BaseAddress = BaseURI;
            client.Timeout = System.Threading.Timeout.InfiniteTimeSpan;
            string requestPath = $"plan/{this.Id}";
            if (this.ConnectionType == ConnectionTypes.sync)
            {
                requestPath += "?c=sync";
            }
            else if (this.ConnectionType == ConnectionTypes.webhook)
            {
                requestPath += "?w=true";
            }
            //validate data
            Stop.ValidateStops(this.Stops);
            Depot.ValidateDepots(this.Depots);
            Vehicle.ValidateVehicles(this.Vehicles);

            //fill in defaults
            foreach (Stop stop in this.Stops)
            {
                if (!stop.From.HasValue)
                {
                    stop.From = this.GeneralSettings.From;
                }
                if (!stop.Till.HasValue)
                {
                    stop.Till = this.GeneralSettings.Till;
                }
                if (string.IsNullOrWhiteSpace(stop.Depot))
                {
                    stop.Depot = this.Depots[0].Name;
                }
            }
            foreach (Vehicle vehicle in this.Vehicles)
            {
                if (!vehicle.AvailFrom.HasValue)
                {
                    vehicle.AvailFrom = this.GeneralSettings.From;
                }
                if (!vehicle.AvailTill.HasValue)
                {
                    vehicle.AvailTill = this.GeneralSettings.Till;
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
                throw new HttpRequestException($"API Return Code {(int)responseMessage.StatusCode}");
            }
            JObject responseObject = JObject.Parse(responseString);
            foreach (JToken stopToken in responseObject["data"]["details"]["stops"])
            {
                Stop receivedStop = stopToken.ToObject<Stop>();
                Stop internalStop = this.Stops.Find(x => x.Name == receivedStop.Name);
                internalStop.Absorb(receivedStop);
            }
            foreach (JToken vehicleToken in responseObject["data"]["details"]["vehicles"])
            {
                Vehicle receivedVehicle = vehicleToken.ToObject<Vehicle>();
                Vehicle internalVehicle = this.Vehicles.Find(x => x.Name == receivedVehicle.Name);
                internalVehicle.Absorb(receivedVehicle);
            }
            foreach (JToken depotToken in responseObject["data"]["details"]["depots"])
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
