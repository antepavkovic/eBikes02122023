using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Bikes.Extensions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Bikes.Models;

namespace Bikes.Controllers
{
    public class HomeController : Controller
    {
        public const string SessionKeyName = "_cart";
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public HomeController(ILogger<HomeController> logger,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

    

        public IActionResult Index(int id)
        {
            return View("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Bike(int? id,int? categoryId)
        {
            List<Bike> bikes= _context.Bike.Where(x => x.Active == true).ToList();


            if (id != null)
            {
                var bike = bikes.Where(p => p.Id == id).FirstOrDefault();
                bike.BikeImages = _context.BikeImage.Where(pi => pi.BikeId == bike.Id).ToList();
                bike.BikeCategories = _context.BikeCategory.Where(pc => pc.BikeId == bike.Id).ToList();
                return View("BikeDetails", bike);
            }

            foreach (var bike in bikes)
            {
                bike.BikeImages = _context.BikeImage.Where(pi => pi.BikeId == bike.Id).ToList();
                bike.BikeCategories = _context.BikeCategory.Where(pi => pi.BikeId == bike.Id).ToList();
            }
            if (categoryId != null)
            {
                bikes = bikes.Where(p => p.BikeCategories.Any(p => p.CategoryId == categoryId)).ToList();

            }
            ViewBag.Categories = _context.Category.ToList();
            return View(bikes);
        }
        [Authorize]

        public IActionResult Order(List<string> errors)
        {
            List<CartItem> cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(SessionKeyName);
            if (cart == null)
            {
                return RedirectToAction("Index");
            }
            if (cart.Count == 0)
            {
                return RedirectToAction("Index");
            }

            for (int i = 0; i < cart.Count; i++)
            {
                var product = _context.Bike.Find(cart[i].Bike.Id);
                if (product == null)
                {
                    cart.RemoveAt(i);
                    i--;
                    errors.Add("Bike not found and was removed from cart!");
                    continue;
                }
                if (product.Quantity < cart[i].Quantity)
                {
                    cart[i].Quantity = product.Quantity;
                    errors.Add("Bike quantity was reduced to available quantity!");
                }
                if (!product.Active)
                {
                    cart.RemoveAt(i);
                    i--;
                    errors.Add("Bike is not active and was removed from cart!");
                    continue;
                }
            }
            HttpContext.Session.SetObjectAsJson(SessionKeyName, cart);

            foreach (CartItem item in cart)
            {
                item.Bike.BikeImages = _context.BikeImage.Where(pi => pi.BikeId == item.Bike.Id).ToList();
                item.Bike.BikeCategories = _context.BikeCategory.Where(pc => pc.BikeId == item.Bike.Id).ToList();
            }

            ViewBag.Errors = errors;

            return View(cart);
        }
        [HttpPost]

        public IActionResult CreateOrder(Order order, bool shippingsameaspersonal)
        {
            List<CartItem> cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(SessionKeyName);
            if (cart == null)
            {
                return RedirectToAction("Index", new { Message = "Cart is empty  so no order can be completed" });

            }
            if (cart.Count == 0)
            {
                return RedirectToAction("Index", new { Message = "Cart is empty  so no order can be completed" });

            }
            var errors = new List<String>();
            for (int i = 0; i < cart.Count; i++)
            {
                var bike = _context.Bike.Find(cart[i].Bike.Id);
                if (bike == null)
                {
                    cart.RemoveAt(i);
                    i--;
                    errors.Add("Bike not found and was removed from cart!");
                    continue;
                }
                if (bike.Quantity < cart[i].Quantity)
                {
                    cart[i].Quantity = bike.Quantity;
                    errors.Add("bike quantity was reduced to available quantity!");
                }
                if (bike.Quantity == 0)
                {
                    cart.RemoveAt(i);
                    i--;
                    errors.Add("Bike" + bike.Name + "is out of stock and it is removed from the cart!");
                }

                if (!bike.Active)
                {
                    cart.RemoveAt(i);
                    i--;
                    errors.Add("Bike" + bike.Name + "is out of stock and it is removed from the cart!");

                    continue;
                }
            }

            HttpContext.Session.SetObjectAsJson(SessionKeyName, cart);
            if (errors.Count > 0)
            {
                return RedirectToAction("Order", new { errors=errors });

            }
            if (shippingsameaspersonal)
            {
                order.ShipppingFirstName = order.BillingFirstName;
                order.ShipppingLastName = order.BillingLastName;
                order.SipppingEmail = order.BillingEmail;
                order.SipppingPhoneNumber = order.BillingPhone;
                order.SipppingAddress = order.BillingAddress;
                order.SipppingCity = order.BillingCity;
                order.SipppingPostalCode = order.BillingPostalCode;
                order.SipppingCountry = order.BillingCountry;
            }
            order.DateCreated = DateTime.Now;
            order.Total = cart.Sum(x => x.Bike.Price * x.Quantity);
            order.UserId = _userManager.GetUserId(User);
            ModelState.Remove("Id");
            ModelState.Remove("OrderItems");
            ModelState.Remove("shippingsameaspersonal");
            if (ModelState.IsValid)
            {
                // TODO:save to database order
                _context.Order.Add(order);
                _context.SaveChanges();
                int order_id = order.Id;
                foreach (var item in cart)
                {
                    OrderItem order_item = new OrderItem()
                    {
                        OrderId = order_id,
                        BikeId = item.Bike.Id,
                        Quantity = item.Quantity,
                        Price = item.Bike.Price
                    };
                    _context.OrderItem.Add(order_item);
                    _context.Bike.Find(item.Bike.Id).Quantity -= item.Quantity;
                    _context.SaveChanges();
                }
                HttpContext.Session.Remove(SessionKeyName);
                return RedirectToAction("Index", new { message = "THank you for your order" });
            }
            else
            {
                errors.Add("Order is not valid!");
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var modelError in modelState.Errors)

                    {
                        errors.Add(modelError.ErrorMessage);
                    }
                }
                if (ModelState.IsValid)
                {
                    // TODO:save to database order
                }
            }
            return RedirectToAction("Order", new { errors=errors });

        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}