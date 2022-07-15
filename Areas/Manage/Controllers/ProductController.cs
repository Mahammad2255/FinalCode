using FinalCode.DAL;
using FinalCode.Extentions;
using FinalCode.Helpers;
using FinalCode.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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

    public class ProductController : Controller
    {
        private readonly FinalCodeDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(FinalCodeDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index(bool? status, int page = 1)
        {
            ViewBag.Status = status;

            IEnumerable<Product> products = await _context.Products
                .Include(t => t.Category)
                .Include(p=>p.ProductSizes).ThenInclude(p=>p.Size)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            ViewBag.PageIndex = page;
            ViewBag.PageCount = Math.Ceiling((double)products.Count() / 5);

            return View(products.Skip((page - 1) * 5).Take(5));
        }
        public async Task<IActionResult> Detail(int? id, bool? status, int page = 1)
        {
            if (id == null) return BadRequest();

            Product product = await _context.Products
               
                .Include(p => p.ProductImages)
                .Include(p => p.ProductSizes).ThenInclude(p => p.Size)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            return View(product);
        }
        public async Task<IActionResult> CreateAsync()
        {
            ViewBag.Categories = await _context.Categories.Where(b => !b.IsDeleted).ToListAsync();
          
            ViewBag.Sizes = await _context.Sizes.Where(b=> !b.IsDeleted).ToListAsync();
  
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(Product product, bool? status, int page = 1)
        {
            ViewBag.Categories = await _context.Categories.Where(b => !b.IsDeleted).ToListAsync();
            ViewBag.Sizes = await _context.Sizes.Where(b => !b.IsDeleted).ToListAsync();


            if (!ModelState.IsValid)
            {
                return View();
            }



            foreach (int item in product.SizeIds)
            {
                if (!await _context.Sizes.AnyAsync(s => s.Id == item))
                {
                    ModelState.AddModelError("", "Incorect Size Id");
                    return View();
                }
            }


            List<ProductSize> productSizes = new List<ProductSize>();

            for (int i = 0; i < product.SizeIds.Count; i++)
            {
                ProductSize productSize = new ProductSize
                {
                    SizeId = product.SizeIds[i]
                };

                productSizes.Add(productSize);
            }

            product.ProductSizes = productSizes;

            

            if (!await _context.Categories.AnyAsync(b => b.Id == product.CategoryId && !b.IsDeleted))
            {
                ModelState.AddModelError("CategoryId", "Duzgun Category Secin ");
                return View();
            }



            if (product.MainImageFile != null)
            {
                if (!product.MainImageFile.CheckFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("MainImageFile", "Secilen Seklin Novu Uygun");
                    return View();
                }

                if (!product.MainImageFile.CheckFileSize(300))
                {
                    ModelState.AddModelError("MainImageFile", "Secilen Seklin Olcusu Maksimum 30 Kb Ola Biler");
                    return View();
                }

                product.MainImage = product.MainImageFile.CreateFile(_env, "assets", "images", "product");
            }
            else
            {
                ModelState.AddModelError("MainImageFile", "Main Sekil Mutleq Secilmelidir");
                return View();
            }


            product.Aviability = product.Count > 0 ? true : false;
            product.CreatedAt = DateTime.UtcNow.AddHours(4);
        

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return RedirectToAction("index", new { status, page });
        }


        //public async Task<IActionResult> GetFormColoRSizeCount()
        //{

        //    ViewBag.Sizes = await _context.Sizes.ToListAsync();

        //    return PartialView("_ProductColorSizePartial");
        //}

        public async Task<IActionResult> Update(int? id, bool? status, int page = 1)
        {
          
            ViewBag.Categories = await _context.Categories.Where(b => !b.IsDeleted).ToListAsync();
            ViewBag.Sizes = await _context.Sizes.Where(t => !t.IsDeleted).ToListAsync();


            if (id == null) return BadRequest();

            Product product = await _context.Products.Include(p => p.ProductSizes).Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();


            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Product product, bool? status, int page = 1)
        {
            //if (!await _context.Categories.AnyAsync(b => b.Id == product.CategoryId && !b.IsDeleted))
            //{
            //    ModelState.AddModelError("CategoryId", "Duzgun Category Secin ");
            //    return View();
            //}

            //ViewBag.Brands = await _context.ProductSizes.Where(b => !b.IsDeleted).ToListAsync();
            ViewBag.Categories = await _context.Categories.Where(b => !b.IsDeleted).ToListAsync();
          
            ViewBag.Sizes = await _context.Sizes.Where(t => !t.IsDeleted).ToListAsync();

            if (id == null) return BadRequest();

            if (id != product.Id) return BadRequest();

            Product dbProduct = await _context.Products
                  .Include(p => p.ProductSizes)
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (dbProduct == null) return NotFound();

            if (!ModelState.IsValid) return View(dbProduct);

           



            if (!await _context.Categories.AnyAsync(b => b.Id == product.CategoryId && !b.IsDeleted))
            {
                ModelState.AddModelError("CategoryId", "Duzgun Category Secin ");
                return View(product);
            }


           
            if (product.MainImageFile != null)
            {
                //if (!product.MainImageFile.CheckFileContentType("image/jpeg"))
                //{
                //    ModelState.AddModelError("MainImageFile", "Secilen Seklin Novu Uygun");
                //    return View();
                //}

                //if (!product.MainImageFile.CheckFileSize(300))
                //{
                //    ModelState.AddModelError("MainImageFile", "Secilen Seklin Olcusu Maksimum 300 Kb Ola Biler");
                //    return View();
                //}
                Helper.DeleteFile(_env, dbProduct.MainImage, "assets", "images", "product");

                dbProduct.MainImage = product.MainImageFile.CreateFile(_env, "assets", "images", "product");
            }

            foreach (int item in product.SizeIds)
            {
                if (!await _context.Sizes.AnyAsync(s => s.Id == item))
                {
                    ModelState.AddModelError("", "Incorect Size Id");
                    return View();
                }
            }


            List<ProductSize> productSizes = new List<ProductSize>();

            for (int i = 0; i < product.SizeIds.Count; i++)
            {
                ProductSize productSize = new ProductSize
                {
                    SizeId = product.SizeIds[i]
                };

                productSizes.Add(productSize);
            }

         
            dbProduct.ProductSizes = productSizes;
            dbProduct.CategoryId = product.CategoryId;
            dbProduct.Price = product.Price;
            dbProduct.DiscountPrice = product.DiscountPrice;
            dbProduct.Description = product.Description;
            dbProduct.Count = product.Count;
            dbProduct.Availability = product.Count > 0 ? true : false;
            dbProduct.UpdatedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            return RedirectToAction("index", new { status, page });
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            Product product = await _context.Products.FirstOrDefaultAsync(s => s.Id == id);
            if (product == null) return NotFound();
            return View(product);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]

        public async Task<IActionResult> DeletePost(int? id)
        {
            if (id == null) return NotFound();
            List<OrderItem> orderItems =  _context.OrderItems.Where(o => o.ProductId == id).ToList();
          
            if(orderItems.Count>0)
                _context.OrderItems.RemoveRange(orderItems);

            List<Basket> baskets = _context.Baskets.Where(b => b.ProductId == id).ToList();

            if (baskets.Count > 0)
                _context.Baskets.RemoveRange(baskets);

            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


    }
}
