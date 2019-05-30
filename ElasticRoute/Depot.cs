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
    /// Represents a depot (warehouse) in a <see cref="Plan"/> your <see cref="Vehicle"/>s start from.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy), ItemNullValueHandling = NullValueHandling.Ignore)]
    public class Depot : Detrack.ElasticRoute.Tools.BaseModel
    {   
        private string _name;
        private string _address;
        private string _postal_code;
        private float? _lat;
        private float? _lng;
        private bool? _default;

        /// <summary>
        /// Gets or sets the depot. Names MUST be distinct within a plan. Required field.
        /// </summary>
        /// <value>Name of the stop. The name must be unique and the value will automatically be trimmed of whitespaces.</value>
        public string Name
        {
            get { return _name; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new BadFieldException("Depot name cannot be null");
                }
                else if (value.Length > 255)
                {
                    throw new BadFieldException("Depot name cannot be more than 255 chars");
                }
                else
                {
                    if (_name != value)
                    {
                        NotifyPropertyChanged();
                        _name = value.Trim();
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets the address of the depot. Required field.
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
        /// Gets or sets the postal code of the depot. This can be used as an alternative form of geocoding (opposed to address/lat+lng) only if your country defined in your plan's GeneralSettings support postal code geocoding.
        /// </summary>
        /// <value>The postal code.</value>
        public string PostalCode
        {
            get => _postal_code;
            set
            {
                if(_postal_code != value)
                {
                    NotifyPropertyChanged();
                    _postal_code = value;
                }
            }

        }
        /// <summary>
        /// Gets or sets the latitude of the depot. Can be used in place of address.
        /// </summary>
        /// <value>The lat.</value>
        public float? Lat
        {
            get => _lat;
            set
            {
                if(_lat != value)
                {
                    NotifyPropertyChanged();
                    _lat = value;
                }
            }
        }
        /// <summary>
        //  Gets or sets the longtitude of the depot. Can be used in place of address.
        /// </summary>
        /// <value>The lng.</value>
        public float? Lng
        {
            get => _lng;
            set
            {
                if(_lng != value)
                {
                    NotifyPropertyChanged();
                    _lng = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets whether this depot would be the depot assumed to be where <see cref="Vehicles"/> without a home depot will start from. If there are multiple depots in the same <see cref="Plan"/> marked as default, the first one will be used.
        /// </summary>
        /// <value>Whether this depot is the default depot</value>
        public bool? Default
        {
            get => _default;
            set
            {
                if(_default != value)
                {
                    NotifyPropertyChanged();
                    _default = value;
                }
            }
        }

        /// <summary>
        /// Initializes a new Depot instance with no forms of address. Use the other constructors to pass addresses. You MUST pass a form of address before solving the plan.
        /// </summary>
        /// <param name="name">Name of the depot. The name must be unique and the value will automatically be trimmed of whitespace.</param>
        [JsonConstructor]
        public Depot(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Initializes a new Depot instance, using a full address to geocode the location.
        /// </summary>
        /// <param name="name">Name of the depot. The name must be unique and the value will automatically be trimmed of whitespace.</param>
        /// <param name="address">Address of the stop. Include the full address with country for the most accurate results.</param>
        public Depot(string name, string address)
        {
            this.Name = name;
            this.Address = address;
        }

        /// <summary>
        /// Initializes a new Depot instance, using a set of coordinates to exactly pinpoint the location.
        /// </summary>
        /// <param name="name">Name of the depot. The name must be unique and the value will automatically be trimmed of whitespace.</param>
        /// <param name="lat">Latitude of the depot</param>
        /// <param name="lng">Longtitude of the depot</param>
        public Depot(string name, float lat, float lng)
        {
            this.Name = name;
            this.Lat = lat;
            this.Lng = lng;
        }

        /// <summary>
        /// Given a list of depots, checks whether all these depots have valid data.
        /// </summary>
        /// <returns><c>true</c>, if all depots were validated, <c>false</c> otherwise.</returns>
        /// <param name="depots">The list of <see cref="Depot"/>s to validate</param>
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

        /// <summary>
        /// Returns a new copy of this instance.
        /// </summary>
        /// <returns>The clone.</returns>
        public Depot Clone()
        {
            string json = JsonConvert.SerializeObject(this);
            return JsonConvert.DeserializeObject<Depot>(json);
        }

        /// <summary>
        /// Copies the attributes of another Depot <paramref name="other"/> such that their public attributes match
        /// </summary>
        /// <param name="other">The other depot to copy from</param>
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
