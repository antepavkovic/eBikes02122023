using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using Bikes.Models;

namespace Bikes.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminBikeImageController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminBikeImageController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AdminBikeImage
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index","AdminBike");
            }
            ViewBag.BikeId = id;
            return _context.BikeImage != null ?
                        View(await _context.BikeImage.Where(x => x.BikeId == id).ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.BikeImage'  is null.");
        }

        // GET: AdminBikeImage/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.BikeImage == null)
            {
                return NotFound();
            }

            var bikeImage = await _context.BikeImage
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bikeImage == null)
            {
                return NotFound();
            }

            return View(bikeImage);
        }

        // GET: AdminBikeImage/Create
        public IActionResult Create(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "AdminBike");

            }
            if (_context.Bike.Count(p => p.Id == id) == 0)
            {
                return RedirectToAction("Index", "AdminBike");
            }
            return View(new BikeImage() { BikeId = (int)id });
        }

        // POST: AdminBikeImage/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
      // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BikeId,IsMainImage,Name,FileName")] BikeImage bikeImage)
        {
            ModelState.Remove("BikeName");
            if (HttpContext.Request.Form.Files.Count > 0)
            {
                ModelState.Remove("FileName");
            }
            if (ModelState.IsValid)
            {


                var imageFile = HttpContext.Request.Form.Files.FirstOrDefault();

                var uploadPath = System.IO.Path.Combine("wwwroot", "images", "products", bikeImage.BikeId.ToString());
                if (!System.IO.Directory.Exists(uploadPath))
                {
                    System.IO.Directory.CreateDirectory(uploadPath);

                }
                if (imageFile != null)
                {
                    var fileName = System.IO.Path.Combine(uploadPath, imageFile.FileName);
                    using (var fileStream = new System.IO.FileStream(fileName, System.IO.FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);

                    }
                    fileName = fileName.Replace("wwwroot\\", "/").Replace("\\", "/");
                    bikeImage.FileName = fileName;
                }
                bikeImage.Id = 0;
                _context.Add(bikeImage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { id = bikeImage.BikeId });
            }
            return View(bikeImage);
        }

        // GET: AdminBikeImage/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.BikeImage == null)
            {
                return NotFound();
            }

            var bikeImage = await _context.BikeImage.FindAsync(id);
            if (bikeImage == null)
            {
                return NotFound();
            }
            return View(bikeImage);
        }

        // POST: AdminBikeImage/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BikeId,IsMainImage,Name,FileName")] BikeImage bikeImage)
        {
            if (id != bikeImage.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bikeImage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BikeImageExists(bikeImage.Id))
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
            return View(bikeImage);
        }

        // GET: AdminBikeImage/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.BikeImage == null)
            {
                return NotFound();
            }

            var bikeImage = await _context.BikeImage
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bikeImage == null)
            {
                return NotFound();
            }

            return View(bikeImage);
        }

        // POST: AdminBikeImage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.BikeImage == null)
            {
                return Problem("Entity set 'ApplicationDbContext.BikeImage'  is null.");
            }
            var bikeImage = await _context.BikeImage.FindAsync(id);
            if (bikeImage != null)
            {
                var fileName = "wwwroot" + bikeImage.FileName.Replace("/", "\\");
                System.IO.File.Delete(fileName);
                _context.BikeImage.Remove(bikeImage);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { id = bikeImage.BikeId });
        }

        private bool BikeImageExists(int id)
        {
          return (_context.BikeImage?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
