using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Assistant.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Assistant.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext()
            : base()
        {
        }
        public Microsoft.EntityFrameworkCore.DbSet<Product> Products { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<List> Lists { get; set; }

        public Microsoft.EntityFrameworkCore.DbSet<ProductList> ProductLists { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            
             optionsBuilder.UseSqlite("Data Source=assistant.db");
           
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ProductList>()
                .HasKey(pl => new { pl.ProductId, pl.ListId });

            modelBuilder.Entity<ProductList>()
                .HasOne(pl => pl.Product)
                .WithMany(p => p.ProductList)
                .HasForeignKey(pl => pl.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductList>()
                .HasOne(pl => pl.List)
                .WithMany(l => l.ProductList)
                .HasForeignKey(pl => pl.ListId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
