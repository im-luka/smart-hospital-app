using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_AI.Hospital.model
{
    [Serializable]
    public class Disease : EntityName
    {
        private bool isVirus;
        private List<Symptom> symptomsList;

        public bool IsVirus { get { return isVirus; } set { isVirus = value; } }
        public List<Symptom> SymptomsList { get { return symptomsList; } set { symptomsList = value; } }

        public Disease(long id, string name, bool isVirus, List<Symptom> symptoms) : base(id, name)
        {
            this.IsVirus = isVirus;
            this.SymptomsList = symptoms;
        }

        public override bool Equals(object obj)
        {
            var disease = obj as Disease;

            if (disease == null || !this.GetType().Equals(obj.GetType()))
                return false;

            return this.Id == disease.Id;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

    }
}
