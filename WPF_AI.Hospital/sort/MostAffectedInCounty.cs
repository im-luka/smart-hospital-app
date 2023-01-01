using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_AI.Hospital.model;

namespace WPF_AI.Hospital.sort
{
    class MostAffectedInCounty : Comparer<County>
    {
        public override int Compare(County county1, County county2)
        {
            //double affectedCounty1 = (double)county1.AffectedPopulation / county1.Population;
            //double affectedCounty2 = (double)county2.AffectedPopulation / county2.Population;

            if (county1.PercentageOfAffectedPopulation < county2.PercentageOfAffectedPopulation)
                return 1;
            else if (county1.PercentageOfAffectedPopulation > county2.PercentageOfAffectedPopulation)
                return -1;
            else
                return 0;
        }
    }
}
