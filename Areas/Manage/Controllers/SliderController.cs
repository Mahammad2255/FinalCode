using FinalCode.DAL;
using FinalCode.Extentions;
using FinalCode.Helpers;
using FinalCode.Models;
using Microsoft.AspNetCore.Authorization;
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
    //[Authorize(Roles = "SuperAdmin,Admin")]

    public class SliderController : Controller
    {
        private readonly FinalCodeDbContext _context;
        private readonly IWebHostEnvironment _env;
        public SliderController(FinalCodeDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Sliders.OrderByDescending(s => s.CreatedAt).ToListAsync());
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(Slider slider)
        {



            if (!ModelState.IsValid) return View();

            slider.Image = slider.SliderImage.CreateFile(_env, "assets", "images", "slider");

            slider.CreatedAt = DateTime.UtcNow.AddHours(4);
            await _context.Sliders.AddAsync(slider);
            await _context.SaveChangesAsync();


            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return NotFound();
            Slider slider = await _context.Sliders.FirstOrDefaultAsync(s => s.Id == id);
            if (slider == null) return NotFound();
            return View(slider);


        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            Slider slider = await _context.Sliders.FirstOrDefaultAsync(s => s.Id == id);
            if (slider == null) return NotFound();
            return View(slider);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]

        public async Task<IActionResult> DeletePost(int? id)
        {
            if (id == null) return NotFound();
            Slider slider = await _context.Sliders.FirstOrDefaultAsync(s => s.Id == id);
            if (slider == null) return NotFound();
            _context.Sliders.Remove(slider);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return NotFound();
            Slider slider = await _context.Sliders.FirstOrDefaultAsync(s => s.Id == id);
            if (slider == null) return NotFound();
            return View(slider);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Slider slider)
        {
            Slider sliderDb = await _context.Sliders.FirstOrDefaultAsync(s => s.Id == id);

            //Slider sliderDb  = await _context.Sliders.FirstOrDefaultAsync();

            if (id == null) return NotFound();
            if (slider == null) return NotFound();
            if (!ModelState.IsValid)
            {
                return View(slider);
            }



            if (slider.SliderImage != null)
            {
                if (!slider.SliderImage.CheckFileContentType("image/png"))
                {
                    ModelState.AddModelError("", "Secilen Seklin Novu Uygun deyil");
                    return View();
                }

                if (!slider.SliderImage.CheckFileSize(30))
                {
                    ModelState.AddModelError("", "Secilen Seklin Olcusu Maksimum 30 Kb Ola Biler");
                    return View();
                }

                Helper.DeleteFile(_env, sliderDb.Image, "assets", "images","slider");

                sliderDb.Image = slider.SliderImage.CreateFile(_env, "assets", "images","slider");
            }

            sliderDb.Title = slider.Title;

            
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));



        }
    }
}
