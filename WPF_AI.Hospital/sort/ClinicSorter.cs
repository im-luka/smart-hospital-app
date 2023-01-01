using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_AI.Hospital.model;

namespace WPF_AI.Hospital.sort
{
    class ClinicSorter : Comparer<Virus>
    {
        public override int Compare(Virus virus1, Virus virus2)
        {
            if (virus1.Name.CompareTo(virus2.Name) > 0)
                return 1;
            else if (virus1.Name.CompareTo(virus2.Name) < 0)
                return -1;
            else
                return 0;
        }
    }
}
