using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Bikes.Models
{
    public class Bike
    {
        [Key]

        public int Id { get; set; }//foreign key na ovaj id se veže još naci **?*

        [Required]


        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; } = true;

        [Required]
        [Column(TypeName = "decimal(9,2)")]
        public decimal Quantity { get; set; }
        [Required]
        [Column(TypeName = "decimal(9,2)")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
        [ForeignKey("BikeId")]
        public virtual ICollection<BikeCategory> BikeCategories { get; set; }
        [ForeignKey("BikeId")]
                           
        public virtual ICollection<OrderItem> OrderItems { get; set; }
        [ForeignKey("BikeId")]
        public virtual ICollection<BikeImage> BikeImages { get; set; }

    }

}
