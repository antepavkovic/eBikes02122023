using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Bikes.Models;

namespace Bikes.Controllers
{
    [Authorize(Roles ="Admin")]

    public class AdminBikeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminBikeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AdminBike
        public async Task<IActionResult> Index(int id)
        {
            if (_context.Bike == null)
            {
                return Problem("Entity set 'ApplicationDbCotext.Bike' is null");
            }
            var products = await _context.Bike.ToListAsync();
            foreach (var product in products)
            {
                product.BikeImages = _context.BikeImage.Where(pi => pi.BikeId == product.Id).ToList();
                product.BikeCategories = _context.BikeCategory.Where(pc => pc.BikeId == product.Id).ToList();
            }
            ViewBag.Categories = _context.Category.ToList();
            return View(products);
        }

        // GET: AdminBike/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Bike == null)
            {
                return NotFound();
            }

            var bike = await _context.Bike
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bike == null)
            {
                return NotFound();
            }

            return View(bike);
        }

        // GET: AdminBike/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdminBike/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Active,Quantity,Price")] Bike bike)
        {
            ModelState.Remove("BikeImages");
            ModelState.Remove("OrderItems");
            ModelState.Remove("BikeCategories");
            if (ModelState.IsValid)
            {
                _context.Add(bike);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(bike);
        }

        // GET: AdminBike/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Bike == null)
            {
                return NotFound();
            }

            var bike = await _context.Bike.FindAsync(id);
            if (bike == null)
            {
                return NotFound();
            }
            bike.BikeImages = _context.BikeImage.Where(pi => pi.BikeId == bike.Id).ToList();
            bike.BikeCategories = _context.BikeCategory.Where(pc => pc.BikeId == bike.Id).ToList();
            ViewBag.Categories = _context.Category.ToList();
            return View(bike);
        }

        // POST: AdminBike/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Active,Quantity,Price")] Bike bike)
        {
            if (id != bike.Id)
            {
                return NotFound();
            }
            ModelState.Remove("BikeImages");
            ModelState.Remove("OrderItems");
            ModelState.Remove("BikeCategories");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bike);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BikeExists(bike.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(bike);
        }
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> EditCategory(int id)
        {
            List<Category> categories = _context.Category.ToList();
            foreach (var cat in categories)
            {
                var checkbox = Request.Form[cat.Name];
                if (checkbox.Contains("true"))
                {
                    if (_context.BikeCategory.FirstOrDefault(x => x.CategoryId == cat.Id && x.BikeId == id) == null)
                        _context.BikeCategory.Add(new BikeCategory() { CategoryId = cat.Id, BikeId = id });
                }
                else
                {
                    var p = _context.BikeCategory.FirstOrDefault(x => x.CategoryId == cat.Id && x.BikeId == id);
                    if (p != null) _context.BikeCategory.Remove(p);
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Edit), new { id });
        }
        public async Task<IActionResult> EditBike(int id)
        {
            List<Category> categories = _context.Category.ToList();
            foreach (var cat in categories)
            {
                var checkbox = Request.Form[cat.Name];
                if (checkbox.Contains("true"))
                {
                    if (_context.BikeCategory.FirstOrDefault(x => x.CategoryId == cat.Id && x.BikeId == id) == null)
                    {
                        _context.BikeCategory.Add(new BikeCategory() { CategoryId = cat.Id, BikeId = id });
                    }

                }
                else
                {
                    var p = _context.BikeCategory.FirstOrDefault(x => x.CategoryId == cat.Id && x.BikeId == id);

                    if (p != null)
                    {
                        _context.BikeCategory.Remove(p);
                    }
                }
            }
            _context.SaveChanges();
            return RedirectToAction(nameof(Edit), new { id });

        }
        // GET: AdminBike/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Bike == null)
            {
                return NotFound();
            }

            var bike = await _context.Bike
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bike == null)
            {
                return NotFound();
            }

            return View(bike);
        }

        // POST: AdminBike/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Bike == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Bike'  is null.");
            }
            var bike = await _context.Bike.FindAsync(id);
            if (bike != null)
            {
                _context.Bike.Remove(bike);
                _context.BikeImage.RemoveRange(_context.BikeImage.Where(pc => pc.BikeId == bike.Id));

            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BikeExists(int id)
        {
          return (_context.Bike?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
