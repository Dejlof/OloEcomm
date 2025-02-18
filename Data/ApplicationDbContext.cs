using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OloEcomm.Model;
using System.Reflection.Emit;

namespace OloEcomm.Data
{
    public class ApplicationDbContext:IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions dbContextOptions):base(dbContextOptions) 
        {
            
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }

        public DbSet<Wishlist> Wishlist { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

       
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
     

        public DbSet<Address> Addresses { get; set; }

        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
  {
      base.OnModelCreating(builder);


      List<IdentityRole> roles = new List<IdentityRole>
      {

          new IdentityRole
          {
              Id ="1",
              Name = "Admin",
              NormalizedName = "ADMIN"
          },

          new IdentityRole
          {
              Id ="2",
              Name ="Vendor",
              NormalizedName = "VENDOR"
          },

          new IdentityRole
          {
              Id="3",
              Name ="Buyer",
              NormalizedName = "BUYER"
          }
      };

            var admin = new User
            {
                Id = "2",
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                NormalizedUserName = "ADMIN@GMAIL.COM",
                NormalizedEmail = "ADMIN@GMAIL.COM",
                FirstName = "Admin",
                LastName = "User",
                Role = "Admin",
                RefreshToken = null,
                RefreshTokenExpiryTime = null,
                EmailConfirmed = true,
                PasswordHash = new PasswordHasher<User>().HashPassword(null, "YourPassword123!"),
                PhoneNumber = "133-476-7890",
                SecurityStamp = Guid.NewGuid().ToString("D")
            };

            var adminUserRole = new IdentityUserRole<string>
            {
                RoleId = "1", 
                UserId = "2"  
            };

            builder.Entity<IdentityRole>().HasData(roles);
            builder.Entity<User>().HasData(admin);
            builder.Entity<IdentityUserRole<string>>().HasData(adminUserRole);

        }

    }
}

