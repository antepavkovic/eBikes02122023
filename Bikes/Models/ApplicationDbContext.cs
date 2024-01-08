using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace Bikes.Models
{
    public class ApplicationUser : IdentityUser
    {
       
        [StringLength(100)]
        public string? FirstName { get; set; }

        [StringLength(100)]
        public string? LastName { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        [NotMapped]
        public string? Role { get; set; }
        [StringLength(200)]
        public string? City { get; set; }

        [StringLength(10)]
        [DataType(DataType.PostalCode)]
        public string? PostalCode { get; set; }
        [StringLength(100)]
        public string? Country { get; set; }
        public string? County { get; set; }
        public override string? Email { get; set; }
        [ForeignKey("UserId")]
        public virtual ICollection<Order>? Orders { get; set; }
    }
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Order> Order { get; set; }
        public DbSet<Bike> Bike { get; set; }
        public DbSet<BikeImage> BikeImage { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<BikeCategory> BikeCategory { get; set; }
        public DbSet<OrderItem> OrderItem { get; set; }

        

    }
}