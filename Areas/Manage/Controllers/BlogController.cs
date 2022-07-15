using FinalCode.DAL;
using FinalCode.Extentions;
using FinalCode.Helpers;
using FinalCode.Models;
using Microsoft.AspNetCore.Hosting;
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

    public class BlogController : Controller
    {
        private readonly FinalCodeDbContext _context;
        private readonly IWebHostEnvironment _env;
        public BlogController(FinalCodeDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Blogs.OrderByDescending(s => s.CreatedAt).ToListAsync());
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(Blog blog)
        {



            if (!ModelState.IsValid) return View();

            blog.Image = blog.BlogImage.CreateFile(_env, "assets", "images", "blog");

            blog.CreatedAt = DateTime.UtcNow.AddHours(4);
            await _context.Blogs.AddAsync(blog);
            await _context.SaveChangesAsync();


            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return NotFound();
            Blog blog = await _context.Blogs.FirstOrDefaultAsync(s => s.Id == id);
            if (blog == null) return NotFound();
            return View(blog);


        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            Blog blog = await _context.Blogs.FirstOrDefaultAsync(s => s.Id == id);
            if (blog == null) return NotFound();
            return View(blog);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]

        public async Task<IActionResult> DeletePost(int? id)
        {
            if (id == null) return NotFound();
            Blog blog = await _context.Blogs.FirstOrDefaultAsync(s => s.Id == id);
            if (blog == null) return NotFound();
            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return NotFound();
            Blog blog = await _context.Blogs.FirstOrDefaultAsync(s => s.Id == id);
            if (blog == null) return NotFound();
            return View(blog);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Blog blog)
        {
            Blog blogDb = await _context.Blogs.FirstOrDefaultAsync(s => s.Id == id);

            //Blog blogDb  = await _context.Blogs.FirstOrDefaultAsync();

            if (id == null) return NotFound();
            if (blog == null) return NotFound();
            if (!ModelState.IsValid)
            {
                return View(blog);
            }



            if (blog.BlogImage != null)
            {
                if (!blog.BlogImage.CheckFileContentType("image/png"))
                {
                    ModelState.AddModelError("", "Secilen Seklin Novu Uygun deyil");
                    return View();
                }

                if (!blog.BlogImage.CheckFileSize(30))
                {
                    ModelState.AddModelError("", "Secilen Seklin Olcusu Maksimum 30 Kb Ola Biler");
                    return View();
                }

                Helper.DeleteFile(_env, blogDb.Image, "assets", "images", "blog");

                blogDb.Image = blog.BlogImage.CreateFile(_env, "assets", "images", "blog");
            }

            blogDb.Title = blog.Title;
            blogDb.Description = blog.Description;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));



        }
    }
}
