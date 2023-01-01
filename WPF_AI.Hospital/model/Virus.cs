using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_AI.Hospital.model
{
    [Serializable]
    public class Virus : Disease
    {
        public Virus(long id, string name, List<Symptom> symptoms) : base(id, name, true, symptoms)
        {

        }

        public override bool Equals(object obj)
        {
            var virus = obj as Virus;

            if (virus == null || !this.GetType().Equals(obj.GetType()))
                return false;

            return this.Id == virus.Id;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}
