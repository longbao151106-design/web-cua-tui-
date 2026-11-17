using System.Data.Entity;
using TraSuaMoc.Models;

namespace TraSuaMoc.Data
{
    public class TeaContext : DbContext
    {
        public TeaContext() : base("name=TeaContext") { }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        protected override void OnModelCreating(DbModelBuilder model)
        {
            model.Entity<Product>().Property(x => x.Price).HasPrecision(18, 2);
            model.Entity<Order>().Property(x => x.Total).HasPrecision(18, 2);
            model.Entity<OrderDetail>().Property(x => x.UnitPrice).HasPrecision(18, 2);
            model.Entity<OrderDetail>().HasRequired(x => x.Product).WithMany().WillCascadeOnDelete(false);
            base.OnModelCreating(model);
        }
    }
}
