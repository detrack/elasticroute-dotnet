using System;
namespace Detrack.ElasticRoute
{
    public class BadFieldException : Exception
    {
        public BadFieldException()
        {
        }

        public BadFieldException(string message)
            : base(message)
        {
        }

        public BadFieldException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
