using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assistant.Models.similarity
{
    public class Similarity
    {
        public string Home { get; set; }
        public string Gender { get; set; }
        public double Salary { get; set; }
        public int Age { get; set; }
        public int Education { get; set; }
        public double Ratio { get; set; }
        public Similarity(string home,string gender,double salary,int education,int age)
        {
            this.Home = home;
            this.Gender = gender;
            this.Salary = salary;
            this.Education = education;
            this.Age = age;
            this.Ratio = Salary * Education;
        }
    }

}
