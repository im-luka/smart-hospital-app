using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_AI.Hospital.model;
using WPF_AI.Hospital.sort;

namespace WPF_AI.Hospital.generics
{
    class ClinicForInfectedDiseases<V, P> where V : Virus where P : Person
    {
        public List<V> VirusList { get; set; }
        public List<P> PersonsList { get; set; }

        public ClinicForInfectedDiseases()
        {
            VirusList = new List<V>();
            PersonsList = new List<P>();
        }

        public ClinicForInfectedDiseases(List<V> virusList, List<P> personsList)
        {
            this.VirusList = virusList;
            this.PersonsList = personsList;
        }

        public void AddVirus(V virus)
        {
            VirusList.Add(virus);
        }

        public void AddPerson(P person)
        {
            PersonsList.Add(person);
        }

        public void PrintViruses()
        {
            Console.WriteLine("List of viruses:");
            VirusList.ForEach(virus =>
            {
                Console.WriteLine(virus.Name.ToString());
            });
        }

        public void PrintPersons()
        {
            Console.WriteLine("List of people affected by viruses:");
            PersonsList.ForEach(person =>
            {
                Console.WriteLine("{0} {1}", person.FirstName, person.LastName);
            });
        }

        public void SortVirus()
        {
            VirusList.Sort(new ClinicSorter());
        }
    }
}
