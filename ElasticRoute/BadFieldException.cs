using System;
namespace Detrack.ElasticRoute
{
    /// <summary>
    /// Exception thrown when you try to set a property to an invalid value
    /// </summary>
    public class BadFieldException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Detrack.ElasticRoute.BadFieldException"/> class.
        /// </summary>
        public BadFieldException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Detrack.ElasticRoute.BadFieldException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        public BadFieldException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Detrack.ElasticRoute.BadFieldException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="inner">Inner.</param>
        public BadFieldException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
