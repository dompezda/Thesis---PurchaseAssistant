using AspNetCore.Identity.MongoDB;
using Assistant.Data;
using Assistant.Models;
using Assistant.Models.similarity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.FileProviders.Physical;
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
        public ApplicationUser CurrentUser;
        //public static ObjectId userId;
        public Product Jaccard(ObjectId listId,ObjectId userId)
        {
            List<SimilarityListObject> JaccardSimilarityList = new List<SimilarityListObject>();
            CurrentUser = db.Users.AsQueryable().Where(x => x.Id == userId.ToString()).FirstOrDefault();
            Product ProdProposition = new Product();
            var UsersList = db.Users.AsQueryable().ToList();

            foreach (var item in UsersList)
            {
                if (item.Id != userId.ToString())
                {
                    SimilarityListObject jaccard = new SimilarityListObject()
                    {
                        userId = ObjectId.Parse(item.Id),
                        similarityValue = 0
                    };
                    JaccardSimilarityList.Add(jaccard);
                }
            }
            foreach (var item in JaccardSimilarityList)
            {
                var GetValue = GetNecessaryData(item.userId,1);
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


        public Product EuclideanDistance(ObjectId listId, ObjectId userId)
        {
            List<SimilarityListObject> EulideanSimilarityList = new List<SimilarityListObject>();
            CurrentUser = db.Users.AsQueryable().Where(x => x.Id == userId.ToString()).FirstOrDefault();
            Product ProdProposition = new Product();
            var UsersList = db.Users.AsQueryable().ToList();
            foreach (var item in UsersList)
            {
                if (item.Id != userId.ToString())
                {
                    SimilarityListObject euclidean = new SimilarityListObject()
                    {
                        userId = ObjectId.Parse(item.Id),
                        similarityValue = 0
                    };
                    EulideanSimilarityList.Add(euclidean);
                }
            }

            foreach (var item in EulideanSimilarityList)
            {
                var GetValue = GetNecessaryData(item.userId,2);
                item.similarityValue = GetValue;
            }
            var Winner = EulideanSimilarityList.OrderBy(x => x.similarityValue).Select(x => x.userId).FirstOrDefault();
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


        public Product AssociationRule(ObjectId listId, ObjectId userId)
        {
            
            Dictionary<ObjectId, int> GetCount = new Dictionary<ObjectId, int>();
            var Prods = db.Products.AsQueryable().ToList();
            var GetList = db.MongoLists.AsQueryable().Where(x => x.Id == listId).FirstOrDefault();
            int counter = Prods.Count;
            List<ObjectId> GetListIds = new List<ObjectId>();
            GetListIds = GetList.ProductList.Select(x => x.Id).ToList();
            for (int i = 0; i < counter; i++)
            {
                if (!GetListIds.Contains(Prods[i].Id))
                {
                    GetCount.Add(Prods[i].Id, 0);
                }
            }
           
            
            if (GetList.ProductList.Count > 0)
            {
                for (int i = 0; i < GetList.ProductList.Count; i++)
                {
                    var ListToCheck = db.MongoLists.AsQueryable().Where(x => x.ProductList.Contains(GetList.ProductList[i])).ToList();
                    for (int y = 0; y < ListToCheck.Count; y++)
                    {
                        var CurrentlyCheckingList = ListToCheck[y];
                        for (int j = 0; j < CurrentlyCheckingList.ProductList.Count; j++)
                        {
                            if (GetCount.Keys.Contains(CurrentlyCheckingList.ProductList[j].Id))
                            {
                                var GetId = CurrentlyCheckingList.ProductList[j].Id;
                                var Value = GetCount.Where(x => x.Key == GetId).Select(x => x.Value);
                                GetCount[GetId] += 1;
                            }

                        }
                    }
                }
            }

            Product ProdProposition = new Product();
            var ObjId=GetCount.OrderBy(x => x.Value).Select(x=>x.Key).Take(1).FirstOrDefault();
            ProdProposition = db.Products.AsQueryable().Where(x => x.Id == ObjId).FirstOrDefault();




            return ProdProposition;
        }




        public double GetNecessaryData(ObjectId userId,int algh)
        {
            double similarity = 0;
            //public Similarity(string home, string gender, double salary, int education, int age)
            //public JaccardSimilarity(string education, string age, string salary, string gender, string home)
            var user = db.Users.AsQueryable().Where(x => x.Id == userId.ToString()).FirstOrDefault();
            //int UserAge = Convert.ToInt32(user.Age);
            //double DoubleSalary = Convert.ToDouble(user.Salary);
            if (algh == 1)
            {
                JaccardSimilarity UserSimilarity = new JaccardSimilarity(user.Education, user.Age, user.Salary, user.Gender, user.Home);
                similarity = CalculateJaccardSimilarity(UserSimilarity, CurrentUser);
            }
            if (algh == 2)
            {
                EuclideanSimilarity UserSimilarity = new EuclideanSimilarity(user.Education, user.Age, user.Salary, user.Gender, user.Home);
                similarity = CalculateEuclideanSimilarity(UserSimilarity, CurrentUser);
            }
            if (algh == 3)
            {
                JaccardSimilarity UserSimilarity = new JaccardSimilarity(user.Education, user.Age, user.Salary, user.Gender, user.Home);
                similarity = CalculateJaccardSimilarity(UserSimilarity, CurrentUser);
            }
            if (algh == 4)
            {
                JaccardSimilarity UserSimilarity = new JaccardSimilarity(user.Education, user.Age, user.Salary, user.Gender, user.Home);
                similarity = CalculateJaccardSimilarity(UserSimilarity, CurrentUser);
            }


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

        public double CalculateEuclideanSimilarity(EuclideanSimilarity UserSimilarity, ApplicationUser user)
        {
            double similarity = 0;
            //age home gender salary education
            double GenderDiff = 0;
            double HomeDiff = GetHomeDiffrenceValue(UserSimilarity.Home, user.Home);
            double AgeDiff=0;
            double SalaryDiff = 0;
            double EducationDiff = 0;
            int UserAge = Convert.ToInt32(user.Age);
            int UserSimilarityAge = Convert.ToInt32(UserSimilarity.Age);
            if (UserSimilarityAge >= UserAge - 5 || UserSimilarityAge <= UserAge + 5)
            {
               AgeDiff = 0;
            }
            else if (UserSimilarityAge >= UserAge - 15 || UserSimilarityAge <= UserAge + 15)
            {
                AgeDiff = 1;
            }
            else
            {
                AgeDiff = 2;
            }
            if (UserSimilarity.Gender != user.Gender)
            {
                GenderDiff = 1;
            };
            if (user.Salary == UserSimilarity.Salary)
            {
                SalaryDiff = 0;
            }
            else
            {
                SalaryDiff = 1;
            }

            int userEducation = GetEducationValue(user.Education);
            int userSimilarityEducation = GetEducationValue(UserSimilarity.Education);
            EducationDiff = userEducation - userSimilarityEducation;
            double GetHomeWeight = HomeWeight();
            double GetAgeWeight = AgeWeight();
            double GetSalaryWeight = SalaryWeight();
            double GetEducationWeight = EducationWeight();
            double GetGenderWeight = GenderWeight();

            similarity = Math.Sqrt(GetAgeWeight * (Math.Pow(AgeDiff, 2)) +
                GetHomeWeight * (Math.Pow(HomeDiff, 2)) +
                GetSalaryWeight * (Math.Pow(SalaryDiff, 2)) +
                GetEducationWeight * (Math.Pow(EducationDiff, 2)) +
                GetGenderWeight * (Math.Pow(GenderDiff, 2)));

            return similarity;
        }
        public double HomeWeight()
        {
            double weight = 0.2;

            return weight;
        }
        public double GenderWeight()
        {
            double weight = 0.2;

            return weight;
        }
        public double SalaryWeight()
        {
            double weight = 0.2;

            return weight;
        }
        public double AgeWeight()
        {
            double weight = 0.2;

            return weight;
        }
        public double EducationWeight()
        {
            double weight = 0.2;

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
        public double GetHomeDiffrenceValue(string userSimilarityHome, string userHome)
        {
            string[] HomeList = new string[17]
            {
            "dolnoslaskie",
            "kujawsko-pomorskie",
            "lubelskie",
            "lubuskie",
            "lodzkie",
            "malopolskie",
            "mazowieckie",
            "opolskie",
            "podkarpackie",
            "podlaskie",
            "pomorskie",
            "slaskie",
            "swietokrzyskie",
            "warminsko-mazurskie",
            "wielkopolskie",
            "zachodniopomorskie",
            "abroad"
            };
            int[,] HomeInt = new int[17, 17]
            {
                {0, 2, 4, 1, 2, 3, 3, 1, 4, 4, 2, 1, 3, 3, 1, 2, 1 },//dolnośląskie
                {2, 0, 2, 2, 1, 3, 1, 2, 3, 2, 1, 2, 2, 1, 1, 2, 2 },//kujawsko-pomorskie
                {4, 2, 0, 4, 2, 2, 1, 3, 1, 2, 3, 2, 1, 2, 3, 4, 1 },//lubelskie
                {1, 2, 4, 0, 2, 4, 3, 2, 4, 4, 2, 3, 3, 2, 1, 1, 1 },//lubuskie
                {2, 1, 2, 2, 0, 2, 1, 1, 2, 2, 2, 1, 1, 2, 1, 2, 2 },//łódzkie
                {3, 3, 2, 4, 2, 0, 2, 2, 1, 3, 4, 1, 1, 3, 3, 4, 1 },//małopolskie
                {3, 1, 1, 3, 1, 2, 0, 2, 2, 1, 2, 2, 1, 1, 2, 3, 2 },//mazowieckie
                {1, 2, 3, 2, 1, 2, 2, 0, 3, 3, 2, 1, 2, 3, 1, 2, 1 },//opolskie
                {4, 3, 1, 4, 2, 1, 2, 3, 0, 3, 4, 2, 1, 3, 3, 4, 1 },//podkarpackie
                {4, 2, 2, 4, 2, 3, 1, 3, 3, 0, 2, 3, 2, 1, 3, 3, 1 },//podlaskie
                {2, 1, 3, 2, 2, 4, 2, 2, 4, 2, 0, 3, 3, 1, 1, 1, 1 },//pomorskie
                {1, 2, 2, 3, 1, 1, 2, 1, 2, 3, 3, 0, 1, 3, 2, 2, 1 },//śląskie
                {3, 2, 1, 3, 1, 1, 1, 2, 1, 2, 3, 1, 0, 2, 2, 3, 2 },//świętokrzyskie
                {3, 1, 2, 2, 2, 3, 1, 3, 3, 1, 1, 3, 2, 0, 2, 2, 1 },//warmińsko-mazurskie
                {1, 1, 3, 1, 1, 3, 2, 1, 3, 3, 1, 2, 2, 2, 0, 1, 2 },//wielkopolskie
                {2, 2, 4, 1, 2, 4, 3, 2, 4, 3, 1, 2, 3, 2, 1, 0, 1 },//zachodniopomorskie
                {1, 2, 1, 1, 2, 1, 2, 1, 1, 1, 1, 1, 2, 1, 2, 1, 0 },//poza granicami Polski
            };

            var userHomeId = Array.FindIndex(HomeList, row => row == userHome);
            var userSimilarityHomeId = Array.FindIndex(HomeList, row => row == userSimilarityHome);
            double Diffrence = HomeInt[userHomeId, userSimilarityHomeId];
            return Diffrence;
        }

    }

}
