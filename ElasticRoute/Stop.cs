#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Detrack.ElasticRoute
{
    /// <summary>
    /// Represents a single stop in a <see cref="Plan"/> you need to route <see cref="Vehicle"/>s to.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy), ItemNullValueHandling = NullValueHandling.Ignore)]
    //[JsonConverter(typeof(Detrack.ElasticRoute.Tools.StopConverter))]
    public class Stop: Detrack.ElasticRoute.Tools.BaseModel 
    {   
        private string _vehicle_type;
        private string _depot;
        private string _name;
        private float _weight_load;
        private float _volume_load;
        private float _seating_load;
        private string _address;
        private string _postal_code;
        private int? _service_time;
        private float? _lat;
        private float? _lng;
        private int? _from;
        private int? _till;
        /// <summary>
        /// Gets or sets the vehicle type of the stop, used to add a constraint where this stop can only be served by vehicles with a corresponding type.
        /// </summary>
        /// <value>The type of the vehicle.</value>
        public string VehicleType
        {
            get => _vehicle_type;
            set
            {
                if (_vehicle_type != value)
                {
                    NotifyPropertyChanged();
                    _vehicle_type = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets the depot of the stop, used to specify the starting location where the driver would pickup the goods/products to be delivered to the stop. If you leave this empty, it will match the first instance in the list of depots in the plan.
        /// </summary>
        /// <value>The depot.</value>
        public string Depot
        {
            get => _depot;
            set
            {
                if(_depot != value)
                {
                    NotifyPropertyChanged();
                    _depot = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the stop. Names MUST be distinct within a plan. Required field.
        /// </summary>
        /// <value>Name of the stop. The name must be unique and the value will automatically be trimmed of whitespace.</value>
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
                    if (_name != value.Trim())
                    {
                        NotifyPropertyChanged();
                        _name = value.Trim();
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets the address of the stop. Required field.
        /// </summary>
        /// <value>The address.</value>
        public string Address
        {
            get => _address;
            set
            {
                if (_address != value)
                {
                    NotifyPropertyChanged();
                    _address = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets the postal code of the stop. This can be used as an alternative form of geocoding (opposed to address/lat+lng) only if your country defined in your plan's GeneralSettings supports postal code geocoding.
        /// </summary>
        /// <value>The postal code.</value>
        public string PostalCode
        {
            get => _postal_code;
            set
            {
                if (_postal_code != value)
                {
                    NotifyPropertyChanged();
                    _postal_code = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets the weight load of the stop, used to add weight contraints to the routing algorithm.
        /// </summary>
        /// <value>The weight load.</value>
        public float WeightLoad
        {
            get { return _weight_load; }
            set
            {
                if (value < 0)
                {
                    throw new BadFieldException("Stop WeightLoad cannot be negative");
                }
                else
                {
                    if (_weight_load != value)
                    {
                        NotifyPropertyChanged();
                        _weight_load = value;
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets the volume load of the stop, used to add volume constraints to the routing algorithm.
        /// </summary>
        /// <value>The volume load.</value>
        public float VolumeLoad
        {
            get { return _volume_load; }
            set
            {
                if (value < 0)
                {
                    throw new BadFieldException("Stop VolumeLoad cannot be negative");
                }
                else
                {
                    if (_volume_load != value)
                    {
                        NotifyPropertyChanged();
                        _volume_load = value;
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets the seating load of the stop, used to add seating constraints to the routing algorithm.
        /// </summary>
        /// <value>The seating load.</value>
        public float SeatingLoad
        {
            get { return _seating_load; }
            set
            {
                if (value < 0)
                {
                    throw new BadFieldException("Stop SeatingLoad cannot be negative");
                }
                else
                {
                    if (_seating_load != value)
                    {
                        NotifyPropertyChanged();
                        _seating_load = value;
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets the service time of the stop, additional time in minutes to be spent at this stop. 
        /// </summary>
        /// <value>The service time.</value>
        public int? ServiceTime
        {
            get => _service_time;
            set
            {
                if (_service_time != value)
                {
                    NotifyPropertyChanged();
                    _service_time = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets the latitude of the stop. Can be used in place of address.
        /// </summary>
        /// <value>The lat.</value>
        public float? Lat
        {
            get => _lat;
            set
            {
                if (_lat != value)
                {
                    NotifyPropertyChanged();
                    _lat = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets the longtitude of the stop. Can be used in place of address.
        /// </summary>
        /// <value>The lng.</value>
        public float? Lng
        {
            get => _lng;
            set
            {
                if (_lng != value)
                {
                    NotifyPropertyChanged();
                    _lng = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets the start of the time window this stop must be served within. Accepts a value of 0 to 2359.
        /// </summary>
        /// <value>From.</value>
        public int? From
        {
            get => _from;
            set
            {
                if (_from != value)
                {
                    NotifyPropertyChanged();
                    _from = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets the end of the time window this stop must be served within. Accepts a value of 0 to 2359.
        /// </summary>
        /// <value>The till.</value>
        public int? Till
        {
            get => _till;
            set
            {
                if(_till != value)
                {
                    NotifyPropertyChanged();
                    _till = value;
                }
            }
        }
        /// <summary>
        /// After solving the plan, gets the name of the vehicle this stop has been assigned to.
        /// </summary>
        /// <value>The assign to.</value>
        [JsonProperty]
        public string AssignTo { get; internal set; }
        /// <summary>
        /// After solving the plan, gets the run number this stop belongs to. A run number is used to identify a sequence of stops served by the same vehicle before returning to the depot.
        /// </summary>
        /// <value>The run.</value>
        [JsonProperty]
        public int Run { get; internal set; }
        /// <summary>
        /// After solving the plan, gets the sequence number of this stop. Denotes the position of this stop in its run. (i.e. the nth stop in the run)
        /// </summary>
        /// <value>The sequence.</value>
        [JsonProperty]
        public int Sequence { get; internal set; }
        /// <summary>
        /// After solving the plan, gets the estimated time of arrival of the serving vehicle.
        /// </summary>
        /// <value>The eta.</value>
        [JsonProperty]
        public DateTime Eta { get; internal set; }
        /// <summary>
        /// After solving the plan, gets the reason why this stop cannot be served.
        /// </summary>
        /// <value>The exception.</value>
        [JsonProperty]
        public string Exception { get; internal set; }

        /// <summary>
        /// Initializes a new Stop instance with no forms of address. Use the other constructors to pass addresses. You MUST pass a form of address before solving the plan.
        /// </summary>
        /// <param name="name">Name of the stop. The name must be unique and the value will automatically be trimmed of whitespace.</param>
        [JsonConstructor]
        public Stop(string name)
        {
            this.Name = name;
        }
        /// <summary>
        /// Initializes a new Stop instance, using a full address to geocode the location.
        /// </summary>
        /// <param name="name">Name of the stop. The name must be unique and the value will automatically be trimmed of whitespace.</param>
        /// <param name="address">Address of the stop. Include the full address with country for the most accurate results.</param>
        public Stop(string name, string address)
        {
            this.Name = name;
            this.Address = address;
        }

        /// <summary>
        /// Initializes a new Stop instance, using a set of coordinates to exactly pinpoint the location.
        /// </summary>
        /// <param name="name">Name of the stop. The name must be unique and the value will automatically be trimmed of whitespace.</param>
        /// <param name="lat">Latitude of the stop</param>
        /// <param name="lng">Longitude of the stop</param>
        public Stop(String name, float lat, float lng)
        {
            this.Name = name;
            this.Lat = lat;
            this.Lng = lng;
        }

        /// <summary>
        /// Given a list of stops, checks whether all these stops have valid data.
        /// </summary>
        /// <returns><c>true</c>, if all stops validated, <c>false</c> otherwise.</returns>
        /// <param name="stops">The list of <see cref="Stop"/>s to validate</param>
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

        /// <summary>
        /// Returns a new copy of this instance.
        /// </summary>
        /// <returns>The clone.</returns>
        public Stop Clone()
        {
            string json = JsonConvert.SerializeObject(this);
            return JsonConvert.DeserializeObject<Stop>(json);
        }

        /// <summary>
        /// Copies the attributes of another Stop <paramref name="other"/> such that their public attributes match.
        /// </summary>
        /// <param name="other">The other stop to copy from</param>
        public void Absorb(Stop other)
        {
            PropertyInfo[] properties = other.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                PropertyInfo internalProperty = this.GetType().GetProperty(property.Name);
                object value = property.GetValue(other);
                if (value != null)
                {
                    Type internalType = internalProperty.PropertyType;
                    Type internalUnderlyingType = Nullable.GetUnderlyingType(internalType);
                    internalProperty.SetValue(this, Convert.ChangeType(value, internalUnderlyingType ?? internalType), null);
                }
                else if (value == null)
                {
                    internalProperty.SetValue(this, null, null);
                }
            }
        }
    }
}
