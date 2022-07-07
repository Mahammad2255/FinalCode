using FinalCode.DAL;
using FinalCode.Extentions;
using FinalCode.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalCode.Areas.Manage.Controllers
{
    [Area("manage")]
    //[Authorize(Roles = "SuperAdmin,Admin")]

    public class CategoryController : Controller
    {
        private readonly FinalCodeDbContext _context;

        public CategoryController(FinalCodeDbContext context)
        {
            _context = context;
          
        }
        public IActionResult Index()
        {
            return View(_context.Categories.ToList());
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(Category category)
        {



            if (!ModelState.IsValid) return View();

            category.CreatedAt = DateTime.UtcNow.AddHours(4);


            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();



            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Update(int? id)
        {
            Category dbCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            return View(dbCategory);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Update(int? id, Category category)
        {
            Category dbCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (!ModelState.IsValid)
            {
                return View();
            }
            if (category.Name.CheckString())
            {
                ModelState.AddModelError("Name", "Yalniz herf ola biler");
                return View(dbCategory);
            }
            if (await _context.Categories.AnyAsync(c => c.Id != id && c.Name.ToLower() == category.Name.ToLower()))
            {
                ModelState.AddModelError("Name", "Category adi movcuddur");
                return View(dbCategory);
            }

            dbCategory.Name = category.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            Category category= await _context.Categories.FirstOrDefaultAsync(s => s.Id == id);
            if (category == null) return NotFound();
            return View(category);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]

        public async Task<IActionResult> DeletePost(int? id)
        {
            if (id == null) return NotFound();
            Category category = await _context.Categories.FirstOrDefaultAsync(s => s.Id == id);
            if (category == null) return NotFound();
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
