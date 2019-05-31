#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator

using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Specialized;

namespace Detrack.ElasticRoute
{
    /// <summary>
    /// Represents a single vehicle in a <see cref="Plan"/> you can use to serve <see cref="Stop"/>s.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy), ItemNullValueHandling = NullValueHandling.Ignore)]
    public class Vehicle : Detrack.ElasticRoute.Tools.BaseModel
    {
        private string _depot;
        private int? _priority;
        private string _name;
        private float? _weight_capacity;
        private float? _volume_capacity;
        private float? _seating_capacity;
        private int? _buffer;
        private int? _avail_from;
        private int? _avail_till;
        private bool? _return_to_depot;
        private ObservableCollection<string> _vehicle_types;
        /// <summary>
        /// Gets or sets the depot of the vehicle, used to specify the "home depot" of this vehicle
        /// </summary>
        /// <value>The depot.</value>
        public string Depot
        {
            get => _depot;
            set
            {
                if (_depot != value)
                {
                    NotifyPropertyChanged();
                    _depot = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets the name of the vehicle. The name MUST be distinct in a plan. Required field.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _name; }
            set
            {
                if (value == null || value.Trim() == "")
                {
                    throw new BadFieldException("Vehicle name cannot be null");
                }
                else if (value.Length > 255)
                {
                    throw new BadFieldException("Vehicle name cannot be more than 255 chars");
                }
                else
                {
                    if (_name != value)
                    {
                        NotifyPropertyChanged();
                        _name = value;
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets the priority of the vehicle. If specified, vehicles with higher priority will be dispatched first.
        /// </summary>
        /// <value>The priority.</value>
        public int? Priority
        {
            get => _priority;
            set
            {
                if (value.HasValue && value < 0)
                {
                    throw new BadFieldException("Vehicle Priority cannot be negative");
                }
                else
                {
                    if (_priority != value)
                    {
                        NotifyPropertyChanged();
                        _priority = value;
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets the weight capacity. If specified, <see cref="Stop.WeightLoad"/> will be considered for route optimization. Leave as <c>null</c> otherwise.
        /// </summary>
        /// <value>The weight capacity.</value>
        public float? WeightCapacity
        {
            get { return _weight_capacity; }
            set
            {
                if (value.HasValue && value < 0)
                {
                    throw new BadFieldException("Vehicle WeightCapacity cannot be negative");
                }
                else
                {
                    if (_weight_capacity != value)
                    {
                        NotifyPropertyChanged();
                        _weight_capacity = value;
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets the volume capacity. If specified, <see cref="Stop.VolumeLoad"/> will be considered for route optimization. Leave as <c>null</c> otherwise.
        /// </summary>
        /// <value>The volume capacity.</value>
        public float? VolumeCapacity
        {
            get { return _volume_capacity; }
            set
            {
                if (value.HasValue && value < 0)
                {
                    throw new BadFieldException("Vehicle VolumeCapacity cannot be negative");
                }
                else
                {
                    if (_volume_capacity != value)
                    {
                        NotifyPropertyChanged();
                        _volume_capacity = value;
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets the seating capacity. If specified, <see cref="Stop.SeatingLoad"/> will be considered for route optimization. Leave as <c>null</c> otherwise.
        /// </summary>
        /// <value>The seating capacity.</value>
        public float? SeatingCapacity
        {
            get { return _seating_capacity; }
            set
            {
                if (value.HasValue && value < 0)
                {
                    throw new BadFieldException("Vehicle SeatingCapacity cannot be negative");
                }
                else
                {
                    if (_seating_capacity != value)
                    {
                        NotifyPropertyChanged();
                        _seating_capacity = value;
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets the buffer time, the amount of travelling time in minutes to factor in for each additional stop.
        /// </summary>
        /// <value>The buffer time in minutes</value>
        public int? Buffer
        {
            get => _buffer;
            set
            {
                if (value.HasValue && value < 0)
                {
                    throw new BadFieldException("Vehicle Buffer cannot be negative");
                }
                else
                {
                    if (_buffer != value)
                    {
                        NotifyPropertyChanged();
                        _buffer = value;
                    }
                }

            }
        }
        /// <summary>
        /// Gets or sets the start of the time window this Vehicle is available to do deliveries. Accepts a value of 0 to 2359.
        /// </summary>
        /// <value>Starting time, int from 0 to 2359</value>
        public int? AvailFrom
        {
            get => _avail_from;
            set
            {
                if (value.HasValue)
                {
                    if (value < 0 || value > 2359)
                    {
                        throw new BadFieldException("Vehicle AvailFrom must be between 0 and 2359");
                    }
                    else
                    {
                        if (_avail_from != value)
                        {
                            NotifyPropertyChanged();
                            _avail_from = value;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets the end of the time window this Vehicle is available to do deliveries. Accepts a value of 0 to 2359.
        /// </summary>
        /// <value>Ending time, int from 0 to 2359</value>
        public int? AvailTill
        {
            get => _avail_till;
            set
            {
                if (value.HasValue)
                {
                    if (value < 0 || value > 2359)
                    {
                        throw new BadFieldException("Vehicle AvailTill must be between 0 and 2359");
                    }
                    else
                    {
                        if (_avail_till != value)
                        {
                            NotifyPropertyChanged();
                            _avail_till = value;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets whether the vehicle should return to the depot after serving all stops. The time required to travel back from the last stop will be taken into account so that the vehicle would return to the depot before the time specified in <see cref="Vehicle.AvailTill"/>.
        /// </summary>
        /// <value>Boolean indicating whether the vehicle should return to the depot or not</value>
        public bool? ReturnToDepot
        {
            get => _return_to_depot;
            set
            {
                if (_return_to_depot != value)
                {
                    NotifyPropertyChanged();
                    _return_to_depot = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets the list of vehicle types this vehicle can be classified under. Used to satisfy constraints set by <see cref="Stop.VehicleType"/>.
        /// </summary>
        /// <value>The vehicle types as a list of strings</value>
        public ObservableCollection<string> VehicleTypes
        {
            get => _vehicle_types;
            set
            {
                if (_vehicle_types != value)
                {
                    _vehicle_types = value;
                    _vehicle_types.CollectionChanged -= vehicleTypesChanged;
                    _vehicle_types.CollectionChanged += vehicleTypesChanged;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Event handler bound to CollectionChanged events
        /// </summary>
        private void vehicleTypesChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            NotifyPropertyChanged("VehicleTypes");
        }

        /// <summary>
        /// Initializes a new Vehicle instance. Name must be distinct.
        /// </summary>
        /// <param name="name">Name of the vehicle. The name must be unique and the value will automatically be trimmed of whitespace.</param>
        [JsonConstructor]
        public Vehicle(string name)
        {
            this.Name = name;
            this.VehicleTypes = new ObservableCollection<string>();
        }

        /// <summary>
        /// Initializes a new Vehicle instance with the home <see cref="Depot"/> explicitly defined. Name must be distinct.
        /// </summary>
        /// <param name="name">Name of the vehicle. The name must be unique and the value will automatically be trimmed of whitespace.</param>
        /// <param name="depot">Home <see cref="Depot"/> of the vehicle. Must match a <see cref="Depot"/> with the same name.</param>
        public Vehicle(string name, string depot)
        {
            this.Name = name;
            this.Depot = depot;
            this.VehicleTypes = new ObservableCollection<string>();
        }

        /// <summary>
        /// Given a list of vehicles, checks whether all these stops have valid data.
        /// </summary>
        /// <returns><c>true</c>, if all vehicles validated, <c>false</c> otherwise.</returns>
        /// <param name="vehicles">Vehicles.</param>
        public static bool ValidateVehicles(List<Vehicle> vehicles)
        {
            if (vehicles.Count == 0)
            {
                throw new BadFieldException("You must have at least one vehicle");
            }
            foreach (Vehicle vehicle in vehicles)
            {
                Predicate<Vehicle> sameNameVehicle = (Vehicle v) => { return v.Name == vehicle.Name; };
                if (vehicles.FindAll(sameNameVehicle).Count > 1)
                {
                    throw new BadFieldException("Vehicle name must be distinct");
                }
            }
            return true;
        }

        /// <summary>
        /// Returns a new copy of this instance
        /// </summary>
        /// <returns>The clone.</returns>
        public Vehicle Clone()
        {
            string json = JsonConvert.SerializeObject(this);
            return JsonConvert.DeserializeObject<Vehicle>(json);
        }

        /// <summary>
        /// Copies the attributes of another <see cref="Vehicle"/> <paramref name="other"/> such that their public attributes match.
        /// </summary>
        /// <param name="other">The other <see cref="Vehicle"/> to copy from</param>
        public void Absorb(Vehicle other)
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
