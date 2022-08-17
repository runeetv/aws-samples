using Microsoft.EntityFrameworkCore;


namespace aurora_postgresql_dotnet.Models
{
    internal class GadgetsOnlineContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnConfiguring (DbContextOptionsBuilder optionsBuilder)
        {
            var connStr = "Host=babelfish-endpoint.cluster-c1234abcdg.us-east-1.rds.amazonaws.com;Database=XXXXX;Username=XXXXX;Password=XXXXX";
            optionsBuilder.UseLazyLoadingProxies();
            optionsBuilder.UseNpgsql(connStr);
        }
        
    }
}
