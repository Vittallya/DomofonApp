using System;
using System.Collections;
using System.Data.Entity;
using System.Linq;
using DAL.Models;

namespace DAL
{
    public class AllDbContext: DbContext
    {        


        public AllDbContext():base("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=DomofonDb;Integrated Security=True")
        {
            
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            
        }

        public DbSet<Models.Client> Clients { get; set; }
        public DbSet<Models.CommonSale> CommonSales { get; set; }
        public DbSet<Models.Order> Orders { get; set; }
        public DbSet<Models.PersonalSale> PersonalSales { get; set; }
        public DbSet<Models.Product> Products { get; set; }
        public DbSet<Models.Profile> Profiles { get; set; }
        public DbSet<Models.Service> Services { get; set; }
        public DbSet<Models.OrderedProduct> OrderedProducts { get; set; }
    }
}
