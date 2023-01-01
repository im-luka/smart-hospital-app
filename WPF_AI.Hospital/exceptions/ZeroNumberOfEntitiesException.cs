using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_AI.Hospital.exceptions
{
    class ZeroNumberOfEntitiesException : Exception
    {
        public ZeroNumberOfEntitiesException() { }

        public ZeroNumberOfEntitiesException(string message) : base(message) { }

        public ZeroNumberOfEntitiesException(string message, Exception exception) : base(message, exception) { }

    }
}
