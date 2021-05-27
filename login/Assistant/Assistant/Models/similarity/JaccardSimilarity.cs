using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assistant.Models.similarity
{
    public class JaccardSimilarity
    {
        public string Home { get; set; }
        public string Gender { get; set; }
        public string Salary { get; set; }
        public string Age { get; set; }
        public string Education { get; set; }
        public JaccardSimilarity(string education, string age, string salary, string gender, string home)
        {
            this.Education = education;
            this.Age = age;
            this.Salary = salary;
            this.Gender = gender;
            this.Home = home;
        }
    }
}
