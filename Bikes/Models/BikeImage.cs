using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Bikes.Models
{

    public class BikeImage
    {
        [Key]
        public int Id { get; set; }

        public int BikeId { get; set; }
        public bool IsMainImage { get; set; } = true;
        [StringLength(200, MinimumLength = 2)]
        public string Name { get; set; }
        [StringLength(500, MinimumLength = 2)]
        [DataType(DataType.ImageUrl)]

        public string FileName { get; set; }
        [NotMapped]
        public string BikeName { get; set; }


    }
}