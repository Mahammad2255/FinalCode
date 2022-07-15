using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinalCode.DAL;
using FinalCode.Enums;
using FinalCode.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalCode.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class OrderController : Controller
    {
        private readonly FinalCodeDbContext _context;

        public OrderController(FinalCodeDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Orders.Include(o=>o.AppUser).Include(o=>o.OrderItems).ToListAsync());
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

            Order order = await _context.Orders
                .Include(o => o.AppUser)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted);

            if (order == null) return NotFound();

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, int orderStatus)
        {
            if (id == null) return BadRequest();

            Order order = await _context.Orders
                .Include(o => o.AppUser)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted);

            if (order == null) return NotFound();

            if (order.Status != OrderStatus.Accepted && orderStatus == 1)
            {
                foreach (var item in order.OrderItems)
                {
                    item.Product.Count -= item.Count;
                }
            }

            order.Status = (OrderStatus)orderStatus;
            order.UpdatedAt = DateTime.UtcNow.AddHours(4);
            await _context.SaveChangesAsync();

            return RedirectToAction("index");
        }

        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            List<OrderItem> orderItems = _context.OrderItems.Where(o => o.OrderId == id).ToList();

            if (orderItems.Count > 0)
                _context.OrderItems.RemoveRange(orderItems);

            Order order = await _context.Orders.FirstOrDefaultAsync(s => s.Id == id);
            if (order == null) return NotFound();

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
