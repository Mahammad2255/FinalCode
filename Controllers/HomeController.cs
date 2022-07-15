
using FinalCode.DAL;
using FinalCode.Models;
using FinalCode.ViewModel;
using FinalCode.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FinalCode.Controllers
{
    public class HomeController : Controller
    {
        private readonly FinalCodeDbContext _context;
        public HomeController(FinalCodeDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            HomeVM homeVM = new HomeVM
            {
                Sliders = _context.Sliders.ToList(),

                Products = _context.Products.OrderByDescending(p => p.Id).Take(4).ToList(),
                Categories = _context.Categories.ToList()
                //Product = _context.Products.Include(r=> r.Reviews).FirstOrDefault()
            };
            return View(homeVM);
        }

        public IActionResult Shop()
        {
            ViewBag.Categories = _context.Categories.Where(b => !b.IsDeleted).ToList();
            ViewBag.isFiltered = false;

            return View(_context.Products.Include(t => t.Category).ToList());
        }

        [HttpPost]
        public IActionResult Shop(List<int> catIds, int minPrice, int maxPrice)
        {
            List<Category> categories = new List<Category>();
            List<Product> products = new List<Product>();
            ViewBag.isFiltered = true;
            string filteredCategories = "";
            ViewBag.Categories = _context.Categories.Where(b => !b.IsDeleted).ToList();

            if (catIds.Count != 0)
            {
                foreach (var categoryId in catIds)
                {
                    Category category = _context.Categories.Where(c => c.Id == categoryId).FirstOrDefault();
                    categories.Add(category);
                }


                foreach (var category in categories)
                {
                    filteredCategories += category.Name;
                    filteredCategories += ", ";
                }

                ViewBag.SelectedCategories = filteredCategories.Trim().Remove(filteredCategories.Trim().Length - 1);

                if (minPrice == 0)
                {
                    if (maxPrice == 0)
                    {
                        foreach (var categoryId in catIds)
                        {
                            Product product = _context.Products.Where(c => c.CategoryId == categoryId).FirstOrDefault();

                            products.Add(product);
                        }
                    }
                    else
                    {
                        foreach (var categoryId in catIds)
                        {
                            Product product = _context.Products.Where(c => c.CategoryId == categoryId && c.Price <= maxPrice).FirstOrDefault();

                            products.Add(product);
                        }
                    }
                }
                else
                {
                    if (maxPrice == 0)
                    {
                        foreach (var categoryId in catIds)
                        {
                            Product product = _context.Products.Where(c => c.CategoryId == categoryId && c.Price >= minPrice).FirstOrDefault();

                            products.Add(product);
                        }
                    }
                    else
                    {
                        foreach (var categoryId in catIds)
                        {
                            Product product = _context.Products.Where(c => c.CategoryId == categoryId && c.Price >= minPrice && c.Price <= maxPrice).FirstOrDefault();

                            products.Add(product);
                        }
                    }
                }
            }
            else
            {
                if (minPrice != 0 && maxPrice != 0)
                    products = _context.Products.Where(c => c.Price >= minPrice && c.Price <= maxPrice).ToList();
                else if (minPrice == 0)
                    products = _context.Products.Where(c => c.Price <= maxPrice).ToList();
                else
                    products = _context.Products.Where(c => c.Price >= minPrice).ToList();
            }

            return View(products);
    }



    }
}
