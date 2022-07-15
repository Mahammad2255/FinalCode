using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinalCode.DAL;
using FinalCode.Models;
using FinalCode.ViewModel.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalCode.Controllers
{
    //[Authorize(Roles ="Member")]
    public class OrderController : Controller
    {
        private readonly FinalCodeDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public OrderController(FinalCodeDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Create()
        {
            AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name && !u.IsAdmin);

            if (appUser == null)
            {
                return RedirectToAction("login", "Account");
            }

            double total = 0;
            List<Basket> baskets = await _context.Baskets.Include(b => b.Product).Where(b => b.AppUserId == appUser.Id).ToListAsync();

            foreach (Basket item in baskets)
            {
                //if(baskets.Count == 1)
                //{
                //    total = total + (item.Count * item.Product.Price);
                //}
                //else
                //{
                //    total = total + item.Product.Price;
                //}

                total = total + (item.Count * item.Product.Price);

            }
            
            ViewBag.Total = total;

            OrderVM orderVM = new OrderVM
            {
                FullName = appUser.FullName,
                Email = appUser.Email,
                Address = appUser.Address,
                City = appUser.City,
                Country = appUser.Country,
                State = appUser.State,
                ZipCode = appUser.ZipCode
            };

            return View(orderVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderVM orderVM, double orderTotal = 0)
        {
            ViewBag.Total = orderTotal;

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Incorect Size Id");
                return View();
            }

            AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name && !u.IsAdmin);

            if (appUser == null)
            {
                return RedirectToAction("login", "Account");
            }

            List<Basket> baskets = await _context.Baskets.Include(b => b.Product).Where(b => b.AppUserId == appUser.Id).ToListAsync();
            List<OrderItem> orderItems = new List<OrderItem>();
            double total = 0;

            foreach (Basket item in baskets)
            {
                if (baskets.Count == 1)
                {
                    total = total + (item.Count * item.Product.Price);
                }
                else
                {
                    total = total + item.Product.Price;
                }

                OrderItem orderItem = new OrderItem
                {
                    Count = item.Count,
                    Price = (item.Product.Price),
                    ProductId = item.ProductId,
                    SizeId = item.SizeId,
                    TotalPrice = (item.Count * (item.Product.Price)),
                    CreatedAt = DateTime.UtcNow.AddHours(4)
                };
                orderItems.Add(orderItem);
            }

            ViewBag.Total = total;



            Order order = new Order
            {
                Address = orderVM.Address,
                AppUserId = appUser.Id,
                City = orderVM.City,
                Country = orderVM.Country,
                State = orderVM.State,
                TotalPrice = total,
                CreatedAt = DateTime.UtcNow.AddHours(4),
                ZipCode = orderVM.ZipCode,
                OrderItems = orderItems
            };

            _context.Baskets.RemoveRange(baskets);
            HttpContext.Response.Cookies.Append("basket", "");
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            return RedirectToAction("index", "home");
        }
    }
}
