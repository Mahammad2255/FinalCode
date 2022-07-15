using FinalCode.ViewModels;
using FinalCode.DAL;
using FinalCode.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinalCode.ViewModel;

namespace FinalCode.Controllers
{
    public class ShopController : Controller
    {
        private readonly FinalCodeDbContext _context;
        public ShopController(FinalCodeDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            IEnumerable<Product> products = await _context.Products
                  .Include(p=>p.Category)
                  .ToListAsync();


            HomeVM homeVM = new HomeVM
            {
                Categories =await _context.Categories.Include(c=>c.Products).Where(p => !p.IsDeleted).ToListAsync(),
                Sizes = await _context.Sizes.Where(p => !p.IsDeleted).ToListAsync(),
                Products=await _context.Products.Where(p => !p.IsDeleted).Include(p=>p.Category).Include(p => p.ProductImages).ToListAsync(),
              
                Reviews=await _context.Reviews.ToListAsync()
            };
            ViewBag.PageIndex = page;
            ViewBag.PageCount = Math.Ceiling((double)products.Count() / 5);
            return View(homeVM);
        }
        public async Task<IActionResult> ProductDetail(int? id)
        {
            if (id == null) return NotFound();
            Product product = await _context.Products.Where(product => product.Id == id && !product.IsDeleted)
                  .Include(p => p.ProductImages)
                  .FirstOrDefaultAsync(p=>p.Id==id);
            if (product == null || product.IsDeleted) return NotFound();
            List<Review> reviews = new List<Review>();

            reviews = await _context.Reviews.Where(p => p.ProductId == product.Id).OrderByDescending(p => p.CreatedAt).ToListAsync();

            HomeVM homeVM = new HomeVM
            {
                Categories = await _context.Categories.Include(c => c.Products).Where(p => !p.IsDeleted).ToListAsync(),
                Sizes = await _context.Sizes.Where(p => !p.IsDeleted).ToListAsync(),
                Products = await _context.Products.Where(p => !p.IsDeleted).Include(p => p.Category).Include(p => p.ProductImages).ToListAsync(),
                Reviews = _context.Reviews.Where(r => r.ProductId == product.Id).OrderByDescending(p=>p.CreatedAt),
                Product =await _context.Products
                .Include(p => p.ProductImages).Include(p => p.ProductSizes).ThenInclude(oi => oi.Size)
                .FirstOrDefaultAsync(p => p.Id == id),
            };

            List<Size> sizes = new List<Size>();
            foreach(var prosize in homeVM.Product.ProductSizes)
            {
                sizes.Add(new Size { Id = prosize.Size.Id, Name = prosize.Size.Name });
            }
            ViewBag.Sizes = sizes;
            return View(homeVM);
        }
     
      
        public async Task<IActionResult> Filter(string filterQuery)
        {
            JObject data = JObject.Parse(filterQuery);
            int catId = (int)data["category"];
            int minPrice = (int)data["minPrice"];
            int maxPrice = (int)data["maxPrice"];
            int brand = (int)data["brand"];
            string type = (string)data["type"];

            IEnumerable<Product> products = new List<Product>();

            products = await _context.Products.Where(
               p => p.Price >= minPrice && p.Price <= maxPrice && p.IsDeleted == false
              ).Include(p => p.ProductImages).ToListAsync();

            if (catId != 0)
            {
                products = products.Where(p => p.CategoryId == catId);
            }
          
            if (type != "standart")
            {
                switch (type)
                {
                    case "ascending":
                        products = products.OrderBy(p => p.Price);
                        break;
                    case "descending":
                        products = products.OrderByDescending(p => p.Price);
                        break;
                    case "latest":
                        products = products.OrderByDescending(p => p.CreatedAt);
                        break;
                    default:
                        break;
                }
            }

            return PartialView("_ShopIndexPartial", products.ToList());
        }

        [HttpPost]
        public async Task<IActionResult> Review(string review)
        {

            JObject data = JObject.Parse(review);

            int productId = (int)data["productId"];
            int rate = (int)data["rate"];
            string message = data["message"].ToString();
            string email = data["email"].ToString();
            string name = data["name"].ToString();

            Product product = await _context.Products.FindAsync(productId);
            if (product == null) return NotFound();

            Review _review = new Review
            {
                Email = email,
                Rate = rate,
                CreatedAt = DateTime.UtcNow,
                Title = message,
                Name = name,
                ProductId = productId
            };

            await _context.Reviews.AddAsync(_review);
            await _context.SaveChangesAsync();

            List<Review> reviews = new List<Review>();

            reviews = await _context.Reviews.Where(p => p.ProductId == productId).OrderByDescending(p => p.CreatedAt).ToListAsync();

            List<ReviewVM> reviewVMs = new List<ReviewVM>();

            reviews.ForEach(r =>
            {
                ReviewVM rVM = new ReviewVM
                {
                    CreatedAt = (DateTime)r.CreatedAt,
                    Star = r.Rate,
                    Name = r.Name,
                    Message = r.Title
                };
                reviewVMs.Add(rVM);
            });

            return PartialView("_ReviewPartial", reviewVMs);

        }
    }
}
