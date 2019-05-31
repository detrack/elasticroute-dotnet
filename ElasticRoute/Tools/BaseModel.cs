using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
namespace Detrack.ElasticRoute.Tools
{
    /// <summary>
    /// Base model used by classes in the <see cref="Detrack.ElasticRoute"/> namespace.
    /// </summary>
    public abstract class BaseModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifies the property changed.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            this.ModifiedProperties.Add(propertyName);
        }
        /// <summary>
        /// A set containing a list of property names that have been modified since the last API call.
        /// </summary>
        [JsonIgnore]
        public HashSet<string> ModifiedProperties = new HashSet<string>();

        /// <summary>
        /// Resets the list of modified properties.
        /// </summary>
        public void ResetModifiedProperties()
        {
            this.ModifiedProperties = new HashSet<string>();
        }
    }
}
