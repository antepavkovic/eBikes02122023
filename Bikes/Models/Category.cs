using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bikes.Models
{
    public class Category
    {
        [Key]

        public int Id { get; set; }
        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string Name { get; set; }
        [ForeignKey("CategoryId")]
        public virtual ICollection<BikeCategory> BikeCategories { get; set; }



    }
}