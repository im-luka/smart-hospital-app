using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_AI.Hospital.model
{
    [Serializable]
    class Person : EntityName
    {
        private string firstName;
        private string lastName;
        private int age;
        private County county;
        private Disease disease;
        private List<Person> contactedPersonsList;

        public string FirstName { get { return firstName; } set { firstName = value; } }
        public string LastName { get { return lastName; } set { lastName = value; } }
        public int Age { get { return age; } set { age = value; } }
        public County County { get { return county; } set { county = value; } }
        public Disease Disease { get { return disease; } set { disease = value; } }
        public List<Person> ContactedPersonsList
        {
            get
            {
                return contactedPersonsList;
            }
            set
            {
                //if (this.Disease is Virus && value != null)
                //{
                //    for (int i = 0; i < value.Count; i++)
                //    {
                //        value[i].Disease = this.Disease;
                //    }
                //}
                contactedPersonsList = value;
            }
        }

        public Person(long id, string firstName, string lastName, int age, County county, Disease disease, List<Person> contactedPersonsList) : base(id, string.Concat(firstName, " ", lastName))
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Age = age;
            this.County = county;
            this.Disease = disease;
            this.ContactedPersonsList = contactedPersonsList;
        }

        public override bool Equals(object obj)
        {
            var person = obj as Person;

            if (person == null || !this.GetType().Equals(obj.GetType()))
                return false;

            return this.FirstName.Equals(person.FirstName) && this.LastName.Equals(person.LastName) && this.Id == person.Id;
        }

        public override int GetHashCode()
        {
            return $"{this.Id} {this.FirstName} {this.LastName}".GetHashCode();
        }

        public class PersonBuilder : EntityName
        {
            private string firstName;
            private string lastName;
            private int age;
            private County county;
            private Disease disease;
            private List<Person> contactedPersonsList;

            public PersonBuilder(long id, string firstName, string lastName) : base(id, string.Concat(firstName, " ", lastName))
            {
                this.firstName = firstName;
                this.lastName = lastName;
            }

            public PersonBuilder HasAge(int age)
            {
                this.age = age;
                return this;
            }

            public PersonBuilder HasCounty(County county)
            {
                this.county = county;
                return this;
            }

            public PersonBuilder HasDisease(Disease disease)
            {
                this.disease = disease;
                return this;
            }

            public PersonBuilder HasContactedPersonsList(List<Person> contactedPeople)
            {
                if (this.disease is Virus && contactedPeople.Count > 0)
                {
                    for (int i = 0; i < contactedPeople.Count; i++)
                    {
                        contactedPeople[i].Disease = this.disease;
                    }
                }
                this.contactedPersonsList = contactedPeople;
                return this;
            }

            public Person Build()
            {
                Person person = new Person(this.Id, this.firstName, this.lastName, this.age, this.county, this.disease, this.contactedPersonsList);
                return person;
            }
        }
    }
}
