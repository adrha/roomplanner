using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomPlanner.Core.Exceptions
{
    public class ForbiddenOperationException : Exception
    {
        public ForbiddenOperationException() { }

        public ForbiddenOperationException(string message) : base(message) { }

        public ForbiddenOperationException(string message, Exception innerException) : base(message, innerException) { }
    }
}
