using Microsoft.EntityFrameworkCore;
namespace LaPizzeria.Models
{
    public class InFornoDbContext : DbContext
    {
        public virtual DbSet<Ingredient> Ingredients { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Checkout> Checkouts { get; set; }
        public InFornoDbContext(DbContextOptions<InFornoDbContext> options) : base(options)
        {
        }
    }
}
