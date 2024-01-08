using Humanizer;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Bikes.Extensions;
using Bikes.Models;

namespace Movies2.Controllers
{
    public class CartController : Controller
    {
        public const string SessionKeyName = "_cart";
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<CartItem> cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(SessionKeyName);
            //ako je cart null instanciramo ga.moramo osigurat i da se barem stvori prazna lista

            if (cart == null)
            {
                cart = new List<CartItem>();

            }
            decimal total = 0;
            foreach (CartItem item in cart)
            {
                item.Bike.BikeImages = _context.BikeImage.Where(pi => pi.BikeId == item.Bike.Id).ToList();
                total += item.GetTtotal();
            }
            ViewBag.CartTotal = total;
            return View(cart);
        }
        [HttpPost]

        public IActionResult AddToCart(int productId)
        {
            List<CartItem> cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(SessionKeyName);
            if (cart == null)
            {
                cart = new List<CartItem>();

            }
            if (cart.Count == 0)//Ukoliko je cart prazan
            {
                CartItem item = new CartItem()
                {
                    Bike = _context.Bike.Find(productId= 1)
                };
                cart.Add(item);
                HttpContext.Session.SetObjectAsJson(SessionKeyName, cart);
            }
            else//cart not  empty,so check if item already exists in cart
            {
                bool found = false;
                foreach (CartItem item in cart)
                {
                    if (item.Bike.Id == productId)
                    {
                        item.Quantity++;
                        found = true;
                        break;

                    }

                }
                if (!found)
                {
                    CartItem item = new CartItem()
                    {
                        Bike = _context.Bike.Find(productId),
                        Quantity = 1

                    };
                    cart.Add(item);
                }
                HttpContext.Session.SetObjectAsJson(SessionKeyName, cart);

            }
            return RedirectToAction("Index");
        }
        public IActionResult RemoveFromCart(int bikeId)
        {
            List<CartItem> cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(SessionKeyName);

            cart.RemoveAll(item => item.Bike.Id == bikeId);

            HttpContext.Session.SetObjectAsJson(SessionKeyName, cart);

            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult ChangeCartItemQuantity(int productId, decimal quantity)
        {
            if (quantity <= 0)
            {
                return RedirectToAction("RemoveFromCart", new { productId = productId });

            }
            List<CartItem> cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(SessionKeyName);
            foreach (CartItem item in cart)
            {
                if (item.Bike.Id == productId)
                {
                    item.Quantity = quantity;
                    break;

                }

            }
            HttpContext.Session.SetObjectAsJson(SessionKeyName, cart);
            return RedirectToAction("Index");

        }
    }
}