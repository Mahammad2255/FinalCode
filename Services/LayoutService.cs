using FinalCode.DAL;
using FinalCode.Models;
using FinalCode.ViewModel.Basket;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalCode.Services
{
    public class LayoutService
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly FinalCodeDbContext _context;

        public LayoutService(IHttpContextAccessor httpContextAccessor, FinalCodeDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public async Task<List<BasketVM>> GetBasket()
        {
            string cookieBasket = _httpContextAccessor.HttpContext.Request.Cookies["basket"];

            List<BasketVM> basketVMs = null;

            if (!string.IsNullOrWhiteSpace(cookieBasket))
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(cookieBasket);
            }
            else
            {
                basketVMs = new List<BasketVM>();
            }

            foreach (BasketVM basketVM in basketVMs)
            {
                Product dbProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketVM.ProductId);
                basketVM.Image = dbProduct.MainImage;
                basketVM.Price =  dbProduct.Price;
                basketVM.EcoTax = dbProduct.EcoTax;
 
                basketVM.Name = dbProduct.Name;
            }

            return basketVMs;
        }

        public async Task<Setting> GetSetting()
        {
            return await _context.Settings.FirstOrDefaultAsync();
        }
    }
  
}
