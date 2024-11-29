using Microsoft.EntityFrameworkCore;
using VäderAppProd.Models;
using VäderAppProd.Models;

namespace VäderAppProd.DataAccess
{
    public class VäderDBContext : DbContext
    {
        public DbSet<WeatherRecord> WeatherData { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=MalekSabriVäder.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WeatherRecord>().ToTable("Prod_WeatherData");
        }
    }
}