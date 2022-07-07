using FinalCode.DAL;
using FinalCode.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalCode.Controllers
{
    public class BlogController: Controller
    {
        private readonly FinalCodeDbContext _context;
        public BlogController(FinalCodeDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {

            return View(_context.Blogs.ToList());
        }
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            Blog blog = await _context.Blogs
                .FirstOrDefaultAsync(b => b.Id == id);

            if (blog == null) return NotFound();

            return View(blog);
        }
    }
}
