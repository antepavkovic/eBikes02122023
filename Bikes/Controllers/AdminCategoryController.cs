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

    public class AdminCategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminCategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AdminCategory
        public async Task<IActionResult> Index()
        {
            return _context.Category != null ? //ako category tablica nije prazna
                        View(await _context.Category.ToListAsync()) ://onda prikaži listu kategorija
                        Problem("Entity set 'ApplicationDbContext.Category'  is null.");//neznam sta ovo radi
        }

      
       
        public IActionResult Create()
        {
            return View();//vraca create view sa title ,create ,back to list(create view)
        }

        // POST: AdminCategory/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Category category)//ovaj create ide tek kada se upise title i stisne na create
        {
            ModelState.Remove("BikeCategories");

            if (ModelState.IsValid)//upisali smo title,izbrisali  product categories i model state je valid
            {
                _context.Add(category);//Upisuje u bazu podataka kategoriju koju smo upisali:product categories = null,title =...,id ide automatski
                await _context.SaveChangesAsync();//spremi to u bazu podataka
                return RedirectToAction(nameof(Index));//vraca na popis kategorija
            }
            return View(category);//View:Category (za ispunit)
        }

        // GET: AdminCategory/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Category == null)//id=7, a movies .models.categories imamo[0]  i[1],dakle 2 titlea
            {
                return NotFound();//views:edit
            }

            var category = await _context.Category.FindAsync(id);//categorija je jednaka onoj kategoriji na koju stisnemo edit
            if (category == null)//False
            {
                return NotFound();//views:edit
            }
            return View(category);//views:edit html
        }

        // POST: AdminCategory/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Category category)
        {
            if (id != category.Id)//id=7 i kategory id =7 dakle ne vraca not found
            {
                return NotFound();
            }
            ModelState.Remove("BikeCategories");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);/*izmjenjuje stanje?*/
                    await _context.SaveChangesAsync();//sprema sve ponovno id,title ,productcategories..
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))//ako kategorija sa Id.iskreno neznam zasto ovo pise
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));//In ASP.NET MVC, the RedirectToAction function is used to switch pages and transfer data between action methods
            }
            return View(category);//vraca categoriju ako nismo uspjeli spremit samo da nesto mora returnat
        }

        // GET: AdminCategory/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Category == null)//id=7, a movies .models.categories imamo[0]  i[1],dakle 2 titlea
            {
                return NotFound();//views:edit nece se izvrsit
            }

            var category = await _context.Category
                .FirstOrDefaultAsync(m => m.Id == id);//categorija je jednaka onoj kategoriji na koju stisnemo delete,//selektira npr.šegrta za brisanje
            if (category == null)
            {
                return NotFound();//views:edit
            }

            return View(category);//views:delete.cshtml
        }

        // POST: AdminCategory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)//ova akcija ide kada stisnemo u viewu delete confimed poslje pitanja
                                                                //are you shure you want to delete this category stisnemo delete
        {
            if (_context.Category == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Category'  is null.");
            }
            var category = await _context.Category.FindAsync(id);//selektira npr.šegrta za brisanje
            if (category != null)
            {
                _context.Category.Remove(category);//briše kategoriju
            }
            
            await _context.SaveChangesAsync();//sprema brisano
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
          return (_context.Category?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
