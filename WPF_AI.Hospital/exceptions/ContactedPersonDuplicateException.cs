using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_AI.Hospital.exceptions
{
    class ContactedPersonDuplicateException : Exception
    {
        public ContactedPersonDuplicateException() { }

        public ContactedPersonDuplicateException(string message) : base(message) { }

        public ContactedPersonDuplicateException(string message, Exception exception) : base(message, exception) { }

    }
}
