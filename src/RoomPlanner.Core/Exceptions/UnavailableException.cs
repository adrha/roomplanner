using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RoomPlanner.Core.Exceptions
{
    public class UnavailableException : Exception
    {
        public UnavailableException()
        {
        }

        public UnavailableException(string? message) : base(message)
        {
        }

        public UnavailableException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UnavailableException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
