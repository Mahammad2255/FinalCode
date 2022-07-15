using FinalCode.DAL;
using FinalCode.Models;
using FinalCode.ViewModel.Basket;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalCode.Controllers
{
    public class ProductController : Controller
    {
        private readonly FinalCodeDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        public ProductController(FinalCodeDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetProductDetail(int? id)
        {
            if (id == null) return BadRequest();

            Product product = await _context.Products.Include(p => p.ProductImages).Include(p => p.ProductSizes).ThenInclude(oi => oi.Size).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();


            return Json(product);
        }

       

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            Product product = await _context.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            ViewBag.Sizes = await _context.Sizes.Where(b => !b.IsDeleted).ToListAsync();

            return PartialView("_ProductDetailPartial", product);
        }


        public async Task<IActionResult> AddBasket(int? id, int sizeId, int count = 1)
        {
            AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name && !u.IsAdmin);

            if (id == null) return BadRequest();

            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            string cookieBasket = HttpContext.Request.Cookies["basket"];

            List<BasketVM> basketVMs = null;

            if (cookieBasket != null && cookieBasket != "")
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(cookieBasket);

                if (basketVMs.Any(b => b.ProductId == id))
                {
                    basketVMs.Find(b => b.ProductId == id).Count += count;
                }
                else
                {
                    basketVMs.Add(new BasketVM
                    {
                        ProductId = (int)id,
                        Count = count,
                        SizeId = sizeId
                    });
                }
            }
            else
            {
                basketVMs = new List<BasketVM>();
                basketVMs.Add(new BasketVM
                {
                    ProductId = (int)id,
                    Count = count,
                    SizeId = sizeId
                });
            }

            cookieBasket = JsonConvert.SerializeObject(basketVMs);
            HttpContext.Response.Cookies.Append("basket", cookieBasket);
            List<Basket> baskets = new List<Basket>();

            foreach (BasketVM basketVM in basketVMs)
            {
                Product dbProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketVM.ProductId);
                basketVM.Image = dbProduct.MainImage;
                basketVM.Price = dbProduct.Price;
                basketVM.Name = dbProduct.Name;

                if (appUser != null)
                {
                    Basket basket = new Basket
                    {
                        AppUserId = appUser.Id,
                        ProductId = basketVM.ProductId,
                        Count = basketVM.Count,
                        SizeId = basketVM.SizeId,
                        CreatedAt = DateTime.UtcNow.AddHours(4)
                    };

                    if (!_context.Baskets.Any(b => b.AppUserId == appUser.Id && b.ProductId == basketVM.ProductId))
                    {
                        baskets.Add(basket);
                    }
                    //else
                    //{
                    //    Basket basket2 = _context.Baskets.Where(b => b.AppUserId == appUser.Id && b.ProductId == id).FirstOrDefault();
                    //    _context.Baskets.Update(basket2);
                    //    await _context.SaveChangesAsync();
                    //}
                }
            }

            if (baskets.Count > 0)
            {
                await _context.Baskets.AddRangeAsync(baskets);
                await _context.SaveChangesAsync();
            }

            return PartialView("_BasketPartial", basketVMs);

        }


        //public async Task<IActionResult> DetailPage(int? id, bool? status, int page = 1)
        //{
        //    if (id == null) return BadRequest();

        //    Product product = await _context.Products

        //        .Include(p => p.ProductImages)
        //        .Include(p => p.ProductSizes).ThenInclude(p => p.Size)
        //        .Include(p => p.Category)
        //        .FirstOrDefaultAsync(p => p.Id == id);

        //    if (product == null) return NotFound();

        //    return View(product);
        //}
    }
}
