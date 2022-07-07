using FinalCode.DAL;
using FinalCode.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using FinalCode.Extentions;
using FinalCode.Helpers;

namespace FinalCode.Areas.Manage.Controllers
{
    [Area("manage")]
   // [Authorize(Roles = "SuperAdmin,Admin")]
    public class SettingController : Controller
    {
        private readonly FinalCodeDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SettingController(FinalCodeDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Settings.FirstOrDefaultAsync());
        }

        public async Task<IActionResult> Detail()
        {
            return View(await _context.Settings.FirstOrDefaultAsync());
        }

        public async Task<IActionResult> Update()
        {
            return View(await _context.Settings.FirstOrDefaultAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Setting setting)
        {

            Setting dbSetting  = await _context.Settings.FirstOrDefaultAsync();

            setting.Logo = dbSetting.Logo;

            if (!ModelState.IsValid)
            {
                return View(setting);
            }



            if (setting.LogoImage != null)
            {
                if (!setting.LogoImage.CheckFileContentType("image/jpg"))
                {
                    ModelState.AddModelError("LogoImage", "Secilen Seklin Novu Uygun deyil");
                    return View();
                }

                if (!setting.LogoImage.CheckFileSize(30))
                {
                    ModelState.AddModelError("LogoImage", "Secilen Seklin Olcusu Maksimum 30 Kb Ola Biler");
                    return View();
                }

                Helper.DeleteFile(_env, dbSetting.Logo, "assets", "images");

                dbSetting.Logo = setting.LogoImage.CreateFile(_env, "assets", "images");
            }

            
           
            dbSetting.UpdatedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            return RedirectToAction("index");
        }
    }
}
