using AspNetCore.Identity.MongoDB;
using Assistant.Data;
using Assistant.Models;
using Assistant.Models.similarity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Assistant.Controllers
{
    public class AlgorithmsController : Controller
    {
        public MongoDbContext db = new MongoDbContext();
        public Dictionary<ObjectId, double> JaccardSimilarityDictionary = new Dictionary<ObjectId, double>();
        public ApplicationUser CurrentUser;
        //public static ObjectId userId;
        public Product Jaccard(ObjectId listId,ObjectId userId)
        {
            List<JaccardListObject> JaccardSimilarityList = new List<JaccardListObject>();
            CurrentUser = db.Users.AsQueryable().Where(x => x.Id == userId.ToString()).FirstOrDefault();
            Product ProdProposition = new Product();
            var UsersList = db.Users.AsQueryable().ToList();

            foreach (var item in UsersList)
            {
                if (item.Id != userId.ToString())
                {
                    JaccardListObject jaccard = new JaccardListObject()
                    {
                        userId = ObjectId.Parse(item.Id),
                        similarityValue = 0
                    };
                    JaccardSimilarityList.Add(jaccard);
                }
            }
            foreach (var item in JaccardSimilarityList)
            {
                var GetValue = GetNecessaryData(item.userId);
                item.similarityValue = GetValue;
            }


            var Winner= JaccardSimilarityList.OrderBy(x => x.similarityValue).Select(x => x.userId).FirstOrDefault();
            //var UserWhoWins = db.Users.AsQueryable().Where(x => x.Id == Winner.ToString()).FirstOrDefault();
            var UserLists = db.MongoLists.AsQueryable().Where(x => x.UserId == Winner).Select(x => x.ProductList).ToList();
            Dictionary<ObjectId, int> GetCount = new Dictionary<ObjectId, int>();
            List<Product> ProdList = db.Products.AsQueryable().ToList();
            foreach (var item in ProdList)
            {
                GetCount.Add(item.Id, 0);
            }
            foreach (var currentList in UserLists)
            {
                foreach (var item in currentList)
                {
                    GetCount[item.Id] += 1;
                }
            }
            var GetFinalProd = GetCount.OrderBy(x => x.Value).Select(x => x.Key).FirstOrDefault();
            ProdProposition = ProdList.Where(x => x.Id == GetFinalProd).FirstOrDefault();
            return ProdProposition;
        }


        public double GetNecessaryData(ObjectId userId)
        {
            //public Similarity(string home, string gender, double salary, int education, int age)
            //public JaccardSimilarity(string education, string age, string salary, string gender, string home)
            var user = db.Users.AsQueryable().Where(x => x.Id == userId.ToString()).FirstOrDefault();
            //int UserAge = Convert.ToInt32(user.Age);
            //double DoubleSalary = Convert.ToDouble(user.Salary);
            JaccardSimilarity UserSimilarity = new JaccardSimilarity(user.Education, user.Age, user.Salary,user.Gender,user.Home);
            double similarity= CalculateJaccardSimilarity(UserSimilarity, CurrentUser);
            
            return similarity;
        }
        public double CalculateJaccardSimilarity(JaccardSimilarity UserSimilarity,ApplicationUser user)
        {
            int UserAge = Convert.ToInt32(user.Age);
            int UserSimilarityAge = Convert.ToInt32(UserSimilarity.Age);
            double similarity = 0;
            if (UserSimilarity.Home == user.Home)
            {
                similarity++;
            }
            //40 50
            if (UserSimilarityAge >= UserAge-10 || UserSimilarityAge <= UserAge+10)
            {
                similarity++;
            }
            if (UserSimilarity.Gender == user.Gender)
            {
                similarity++;
            }
            if (UserSimilarity.Salary == user.Salary)
            {
                similarity++;
            }
            if (UserSimilarity.Education == user.Education)
            {
                similarity++;
            }
            return similarity;
        }






        public double HomeWeight(string Home)
        {
            double weight = 0;

            return weight;
        }
        public double GenderWeight(string Home)
        {
            double weight = 0;

            return weight;
        }
        public double SalaryWeight(string Home)
        {
            double weight = 0;

            return weight;
        }
        public double AgeWeight(string Home)
        {
            double weight = 0;

            return weight;
        }
        public double EducationWeight(string Home)
        {
            double weight = 0;

            return weight;
        }
        public int GetEducationValue(string education)
        {
            Dictionary<string, int> Values = new Dictionary<string, int>();
            Values.Add("podstawowe", 1);
            Values.Add("gimnazjalne", 2);
            Values.Add("zasadnicze zawodowe", 3);
            Values.Add("zasadnicze branzowe", 4);
            Values.Add("srednie branzowe", 5);
            Values.Add("srednie", 6);
            Values.Add("wyzsze", 7);

            int Value = Values.Where(x => x.Key == education).Select(x => x.Value).FirstOrDefault();
            return Value;
        }
    }
}
