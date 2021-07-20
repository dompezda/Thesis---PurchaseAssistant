using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assistant.Data;
using Assistant.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Assistant.Data
{
    public class MongoDbContext //IdentityDbContext<ApplicationUser>
    {
        private MongoClient ClinetMongo;
        public IMongoCollection<Product> Products;
        public IMongoCollection<ProductList> ProductList; //useless
        public IMongoCollection<ListOfProducts> List;
        public IMongoCollection<ApplicationUser> Users;
        public IMongoCollection<MongoDBProdList> MongoLists;
        public IMongoDatabase db;

        public MongoDbContext()
        {
             string _user = "****";
             string _password = "****";
             string _database = "****";

            string _connectionstring = $"mongodb+srv://{_user}:{_password}@cluster0.od5jh.mongodb.net/{_database}?retryWrites=true&w=majority";
            //string _connectionstring = $"mongodb + srv://cluster0.od5jh.mongodb.net/{_database}?authSource=%24external&authMechanism=MONGODB-X509&retryWrites=true&w=majority";
            ClinetMongo = new MongoClient(_connectionstring);
            db = ClinetMongo.GetDatabase("PAssistant");
            Products = db.GetCollection<Product>("Products");
            List = db.GetCollection<ListOfProducts>("List");
            ProductList = db.GetCollection<ProductList>("ProductList"); //useless
            Users = db.GetCollection<ApplicationUser>("Users");
            MongoLists = db.GetCollection<MongoDBProdList>("List");
        }
        
        //MyMongoDatabase
       
      
    }
}
