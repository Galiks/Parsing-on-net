using Microsoft.EntityFrameworkCore;
using Parsing_on_.net.Models;

namespace Parsing_on_.net.DAL
{
    public class ShopContext : DbContext
    {
        public DbSet<Shop> Shops { get; set; }
        public DbSet<Timer> Timers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("server=localhost;user id=root;persistsecurityinfo=True;database=forvisualstudio;password=admin");
        }
    }
}