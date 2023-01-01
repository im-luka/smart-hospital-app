using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_AI.Hospital.exceptions
{
    class InputGreaterThanOptionsException : Exception
    {
        public InputGreaterThanOptionsException() { }

        public InputGreaterThanOptionsException(string message) : base(message) { }

        public InputGreaterThanOptionsException(string message, Exception exception) : base(message, exception) { }

    }
}
