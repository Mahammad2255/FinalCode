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
    [Authorize(Roles = "SuperAdmin,Admin")]

    public class SizeController : Controller
    {
        private readonly FinalCodeDbContext _context;

        public SizeController(FinalCodeDbContext context)
        {
            _context = context;
          
        }
        public async Task<IActionResult> Index(bool? status, int page = 1)
        {
            ViewBag.Status = status;

            IEnumerable<Size> Sizes = await _context.Sizes
                .Include(t => t.ProductSizes)
                .Where(t => status != null ? t.IsDeleted == status : true)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            ViewBag.PageIndex = page;
            ViewBag.PageCount = Math.Ceiling((double)Sizes.Count() / 5);

            return View(Sizes.Skip((page - 1) * 5).Take(5));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Size size)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (string.IsNullOrWhiteSpace(size.Name))
            {
                ModelState.AddModelError("Name", "Bosluq Olmamalidir");
                return View();
            }

            //tag.Name = tag.Name.Trim();

            if (size.Name.CheckString())
            {
                ModelState.AddModelError("Name", "Yalniz Herf Ola Biler");
                return View();
            }

            if (await _context.Tags.AnyAsync(t => t.Name.ToLower() == size.Name.ToLower()))
            {
                ModelState.AddModelError("Name", "Alreade Exists");
                return View();
            }

            size.CreatedAt = DateTime.UtcNow.AddHours(4);

            await _context.Sizes.AddAsync(size);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }



        //public async Task<IActionResult> Create(Size size)
        //{



        //    if (!ModelState.IsValid) return View();

        //    //size.CreatedAt = DateTime.UtcNow.AddHours(4);


        //    await _context.Sizes.AddAsync(size);
        //    await _context.SaveChangesAsync();



        //    return RedirectToAction("Index");
        //}

        public async Task<IActionResult> Update(int? id, bool? status, int page = 1)
        {
            if (id == null) return BadRequest();

            Size size = await _context.Sizes.FirstOrDefaultAsync(t => t.Id == id);

            if (size == null) return NotFound();

            return View(size);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Size size, bool? status, int page = 1)
        {
            if (!ModelState.IsValid) return View(size);

            if (id == null) return BadRequest();

            if (id != size.Id) return BadRequest();

            Size dbSize = await _context.Sizes.FirstOrDefaultAsync(t => t.Id == id);

            if (dbSize == null) return NotFound();

            if (string.IsNullOrWhiteSpace(size.Name.ToString()))
            {
                ModelState.AddModelError("Name", "Bosluq Olmamalidir");
                return View(size);
            }

            if (size.Name.CheckNumber())
            {
                ModelState.AddModelError("Name", "Yalniz eded ola biler");
                return View(size);
            }

            if (await _context.Sizes.AnyAsync(s => s.Id != size.Id && s.Name.ToLower() == size.Name.ToLower()))
            {
                ModelState.AddModelError("Name", "Daxil etdiyiniz olcu movcuddur");
                return View(size);
            }

            dbSize.Name = size.Name;
            dbSize.UpdatedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { status = status, page = page });
        }

        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null) return NotFound();
        //    Size size = await _context.Sizes.FirstOrDefaultAsync(s => s.Id == id);
        //    if (size == null) return NotFound();
        //    return View(size);

        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[ActionName("Delete")]

        //public async Task<IActionResult> DeletePost(int? id)
        //{
        //    if (id == null) return NotFound();
        //    Size size = await _context.Sizes.FirstOrDefaultAsync(s => s.Id == id);
        //    if (size == null) return NotFound();
        //    _context.Sizes.Remove(size);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}






        public async Task<IActionResult> Delete(int? id, bool? status, int page = 1)
        {
            if (id == null) return BadRequest();

            Size dbSize = await _context.Sizes.FirstOrDefaultAsync(t => t.Id == id);

            if (dbSize == null) return NotFound();

            dbSize.IsDeleted = true;
            dbSize.DeletedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            ViewBag.Status = status;

            IEnumerable<Size> sizes = await _context.Sizes
                .Include(t => t.ProductSizes)
                .Where(t => status != null ? t.IsDeleted == status : true)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            ViewBag.PageIndex = page;
            ViewBag.PageCount = Math.Ceiling((double)sizes.Count() / 5);



            return PartialView("_TagIndexPartial", sizes.Skip((page - 1) * 5).Take(5));
        }

        public async Task<IActionResult> Restore(int? id, bool? status, int page = 1)
        {
            if (id == null) return BadRequest();

            Size dbSize = await _context.Sizes.FirstOrDefaultAsync(t => t.Id == id);

            if (dbSize == null) return NotFound();

            dbSize.IsDeleted = false;
            dbSize.DeletedAt = null;

            await _context.SaveChangesAsync();

            ViewBag.Status = status;

            IEnumerable<Size> sizes = await _context.Sizes
                .Include(t => t.ProductSizes)
                .Where(t => status != null ? t.IsDeleted == status : true)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            ViewBag.PageIndex = page;
            ViewBag.PageCount = Math.Ceiling((double)sizes.Count() / 5);



            return PartialView("_TagIndexPartial", sizes.Skip((page - 1) * 5).Take(5));
        }
    }
}
