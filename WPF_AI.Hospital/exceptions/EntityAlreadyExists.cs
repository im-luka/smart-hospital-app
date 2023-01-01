using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_AI.Hospital.exceptions
{
    class EntityAlreadyExists : Exception
    {
        public EntityAlreadyExists() { }

        public EntityAlreadyExists(string message) : base(message) { }

        public EntityAlreadyExists(string message, Exception exception) : base(message, exception) { }

    }
}
