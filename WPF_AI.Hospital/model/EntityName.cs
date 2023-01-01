using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_AI.Hospital.model
{
    public abstract class EntityName
    {
        private long id;
        private string name;

        public long Id { get { return id; } set { id = value; } }
        public string Name { get { return name; } set { name = value; } }

        public EntityName(long id, string name)
        {
            this.Id = id;
            this.Name = name;
        }
    }
}
