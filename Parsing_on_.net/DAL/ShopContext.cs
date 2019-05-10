using Parsing_on_.net.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Parsing_on_.net.DAL
{
    public class ShopContext : DbContext
    {
        public DbSet<Shop> Shops { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}