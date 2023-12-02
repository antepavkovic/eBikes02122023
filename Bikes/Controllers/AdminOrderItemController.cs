using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Bikes.Models;

namespace Movies2.Controllers
{
    [Authorize(Roles = "Admin")]

    public class AdminOrderItemController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminOrderItemController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AdminOrderItem
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("index", "AdminOrder");
            }
            var orderItems = await _context.OrderItem.Where(o => o.OrderId == id).ToListAsync();
            foreach (var item in orderItems)
            {
                item.BikeName = (from bike in _context.Bike
                                     where bike.Id == item.BikeId
                                     select bike.Name).FirstOrDefault();


            }
            return View(orderItems);
        }
        // GET: AdminOrderItem/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.OrderItem == null)
            {
                return NotFound();
            }

            var orderItem = await _context.OrderItem
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderItem == null)
            {
                return NotFound();
            }
            orderItem.BikeName= (from bike in _context.Bike
                                      where bike.Id == orderItem.BikeId
                                      select bike.Name).FirstOrDefault();

            return View(orderItem);
        }

        // GET: AdminOrderItem/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdminOrderItem/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OrderId,BikeId,Quantity,Price")] OrderItem orderItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(orderItem);
        }

        // GET: AdminOrderItem/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.OrderItem == null)
            {
                return NotFound();
            }

            var orderItem = await _context.OrderItem.FindAsync(id);
            if (orderItem == null)
            {
                return NotFound();
            }
            orderItem.BikeName = (from bike in _context.Bike
                                      where bike.Id == orderItem.BikeId
                                      select bike.Name).FirstOrDefault();
            return View(orderItem);
        }

        // POST: AdminOrderItem/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrderId,BikeId,Quantity,Price")] OrderItem orderItem)
        {
            if (id != orderItem.Id)
            {
                return NotFound();
            }
            if (orderItem.Price <= 0)
            {
                ModelState.AddModelError("Price", " price must be greater than 0");
            }
            if (orderItem.Quantity <= 0)
            {
                ModelState.AddModelError("Qunatity", " Qunatity must be greater than 0");
            }
            ModelState.Remove("BikeName");
            if (ModelState.IsValid)
            {
                try
                {
                    var old_orderItem = await _context.OrderItem.FindAsync(id);
                    var quantity_diff = orderItem.Quantity - old_orderItem.Quantity;
                    var price_diff = orderItem.Price - old_orderItem.Price;
                    if (quantity_diff < 0)
                    {
                        _context.Bike.Find(orderItem.BikeId).Quantity += Math.Abs(quantity_diff);
                    }
                    if (quantity_diff > 0)
                    {
                        var availabile_quantity = _context.Bike.Find(orderItem.BikeId).Quantity;
                        if (availabile_quantity < quantity_diff)
                        {
                            ModelState.AddModelError("Quantity", $"Only {availabile_quantity} " +
                                $"items are available,you tried to add {quantity_diff}");
                            orderItem.BikeName = (from bike in _context.Bike
                                                      where bike.Id == orderItem.BikeId
                                                      select bike.Name).FirstOrDefault();
                            return View(orderItem);
                        }
                        _context.Bike.Find(orderItem.BikeId).Quantity -= quantity_diff;

                    }
                    if (price_diff != 0 || quantity_diff != 0)
                    {
                        var oldprice = old_orderItem.Price * old_orderItem.Quantity;
                        var new_price = orderItem.Price * orderItem.Quantity;


                        _context.Order.Find(orderItem.OrderId).Total += new_price - oldprice;

                    }
                    old_orderItem.Quantity = orderItem.Quantity;
                    old_orderItem.Price = orderItem.Price;
                    // _context.Update(orderItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderItemExists(orderItem.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { id = orderItem.OrderId });
            }
            orderItem.BikeName = (from bike in _context.Bike
                                  where bike.Id == orderItem.BikeId
                                  select bike.Name).FirstOrDefault();
            return View(orderItem);
        }

        // GET: AdminOrderItem/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.OrderItem == null)
            {
                return NotFound();
            }

            var orderItem = await _context.OrderItem
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderItem == null)
            {
                return NotFound();
            }
            orderItem.BikeName = (from Bike in _context.Bike
                                      where Bike.Id == orderItem.BikeId
                                      select Bike.Name).FirstOrDefault();
            return View(orderItem);
        }

        // POST: AdminOrderItem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.OrderItem == null)
            {
                return Problem("Entity set 'ApplicationDbContext.OrderItem'  is null.");
            }
            var orderItem = await _context.OrderItem.FindAsync(id);
            int? orderid = orderItem.OrderId;
            if (orderItem != null)
            {
                _context.Bike.Find(orderItem.BikeId).Quantity += orderItem.Quantity;
                _context.Order.Find(orderItem.OrderId).Total -= orderItem.Price * orderItem.Quantity;
                _context.OrderItem.Remove(orderItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { id = orderid });



        }
        bool OrderItemExists(int id)
        {
            return (_context.OrderItem?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}