using Bikes.Models;

namespace Bikes.Extensions
{
    public class CartItem//ono što nam se nalazi u košarici
    {
        public Bike Bike { get; set; }
        public decimal Quantity { get; set; }
        public decimal GetTtotal()
        {
            return Bike.Price * Quantity;

        }


    }
}