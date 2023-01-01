using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_AI.Hospital.model
{
    [Serializable]
    class County : EntityName
    {
        private int population;
        private int affectedPopulation;

        public int Population { get { return population; } set { population = value; } }
        public int AffectedPopulation { get { return affectedPopulation; } set { affectedPopulation = value; } }
        public double PercentageOfAffectedPopulation { get; set; }

        public County(long id, string name, int population, int affectedPopulation) : base(id, name)
        {
            this.Population = population;
            this.AffectedPopulation = affectedPopulation;
            this.PercentageOfAffectedPopulation = ((double)AffectedPopulation / Population) * 100;
        }

        public override bool Equals(object obj)
        {
            var county = obj as County;

            if (county == null || !this.GetType().Equals(obj.GetType()))
                return false;

            return this.Id == county.Id;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}
