using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductGuide1._0.Model;

namespace ProductGuide1._0
{
    internal class ApplicationContext : DbContext
    {
        public DbSet<PriceProduct> Prices { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=productsDB.db");
        }
    }
}
