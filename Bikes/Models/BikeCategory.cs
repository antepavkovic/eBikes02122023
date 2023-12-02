using System.ComponentModel.DataAnnotations.Schema;

namespace Bikes.Models
{

    public class BikeCategory
    {
        public int Id { get; set; }
        public int BikeId { get; set; }
        public int CategoryId { get; set; }
        [NotMapped]
        public string BikeName { get; set; }
        [NotMapped]
        public string CategoryTitle { get; set; }




    }
}