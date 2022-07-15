using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using FinalCode.DAL;
using FinalCode.Models;
using FinalCode.ViewModel.Account;
using FinalCode.ViewModel.Basket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;
using FinalCode.ViewModel;

namespace FinalCode.Controllers
{
    public class AccountController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly FinalCodeDbContext _context;
        private readonly IConfiguration _config;

        public AccountController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, IConfiguration config, SignInManager<AppUser> signInManager, FinalCodeDbContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _config = config;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid) return View();

            AppUser appUser = new AppUser
            {
                FullName = registerVM.FullName,
                Email = registerVM.Email,
                UserName = registerVM.UserName,
                IsAdmin = false
            };
            string token = Guid.NewGuid().ToString();
            appUser.EmailConfirmationToken = token;
            IdentityResult identityResult = await _userManager.CreateAsync(appUser, registerVM.Password);

            if (!identityResult.Succeeded)
            {
                foreach (var item in identityResult.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View();
            }

            await _userManager.AddToRoleAsync(appUser, "Member");

            var link = Url.Action(nameof(VerifyEmail), "Account", new { id = appUser.Id, token }, Request.Scheme, Request.Host.ToString());

            EmailVM email = _config.GetSection("Email").Get<EmailVM>();
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(email.SenderEmail, email.SenderName);
            mail.To.Add(appUser.Email);
            mail.Subject = "VerifyEmail";
            mail.Body = $"<a href=\"{link}\">Verify</a>";
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = email.Server;
            smtp.Port = email.Port;
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential(email.SenderEmail, email.Password);
            smtp.Send(mail);
            //await _signInManager.SignInAsync(appUser, isPersistent:false);

            return RedirectToAction(nameof(EmailVerification));


        }

        public IActionResult EmailVerification() => View();



        public async Task<IActionResult> VerifyEmail(string id, string token)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            AppUser user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            if (user.EmailConfirmationToken != token)
            {
                return BadRequest();
            }

            var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            IdentityResult result = await _userManager.ConfirmEmailAsync(user, emailConfirmationToken);

            if (result.Succeeded)
            {
                string newToken = Guid.NewGuid().ToString();
                user.EmailConfirmationToken = newToken;
                await _userManager.UpdateAsync(user);
                return View();
            }



            return BadRequest();
        }
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("index", "home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid) return View();

            AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.NormalizedEmail == loginVM.Email.ToUpper() && !u.IsAdmin);

            if (appUser == null)
            {
                ModelState.AddModelError("", "Email Or Password Is InCorrect");
                return View();
            }
            Microsoft.AspNetCore.Identity.SignInResult signInResult = await _signInManager.PasswordSignInAsync(appUser, loginVM.Password, loginVM.RememberMe, true);

            if (!signInResult.Succeeded)
            {
                ModelState.AddModelError("", "Email Or Password Is InCorrect");
                return View();
            }

            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelError("", "Hesabiniz Blocklanib");
                return View();
            }

            string coockieBasket = HttpContext.Request.Cookies["basket"];

            if (!string.IsNullOrWhiteSpace(coockieBasket))
            {
                List<BasketVM> basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(coockieBasket);

                List<Basket> baskets = new List<Basket>();
                List<Basket> existedBasket = await _context.Baskets.Where(b => b.AppUserId == appUser.Id).ToListAsync();
                foreach (BasketVM basketVM in basketVMs)
                {
                    if (existedBasket.Any(b => b.ProductId == basketVM.ProductId))
                    {
                        existedBasket.Find(b => b.ProductId == basketVM.ProductId).Count = basketVM.Count;
                    }
                    else
                    {
                        if (!_context.Baskets.Any(b => b.AppUserId == appUser.Id && b.ProductId == basketVM.ProductId))
                        {
                            Basket basket = new Basket
                            {
                                AppUserId = appUser.Id,
                                ProductId = basketVM.ProductId,
                                SizeId = basketVM.SizeId,
                                Count = basketVM.Count,
                                CreatedAt = DateTime.UtcNow.AddHours(4)
                            };

                            baskets.Add(basket);
                        }
                    }


                }

                if (baskets.Count > 0)
                {
                    await _context.Baskets.AddRangeAsync(baskets);
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction("index", "home");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }

        //[Authorize(Roles = "Member")]
        public async Task<IActionResult> Profile()
        {
            AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name && !u.IsAdmin);

            if (appUser == null) return RedirectToAction("index", "home");

            MemberProfileVM memberProfileVM = new MemberProfileVM
            {
                Member = new MemberUpdateVM
                {
                    Address = appUser.Address,
                    City = appUser.City,
                    Country = appUser.Country,
                    FullName = appUser.FullName,
                    PhoneNumber = appUser.PhoneNumber,
                    State = appUser.State,
                    UserName = appUser.UserName,
                    ZipCode = appUser.ZipCode,
                    Email = appUser.Email
                },
                Orders = await _context.Orders
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Size)
                .Include(o => o.AppUser)
                .Where(o => !o.IsDeleted && o.AppUserId == appUser.Id).ToListAsync()

            };

            return View(memberProfileVM);
        }

        [Authorize(Roles = "Member")]
        [HttpPost]
        public async Task<IActionResult> Edit(MemberUpdateVM member)
        {
            AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name && !u.IsAdmin);

            if (appUser == null) return RedirectToAction("index", "home");
            MemberProfileVM memberProfileVM = new MemberProfileVM
            {
                Member = member
            };
            TempData["ProfileTab"] = "Account";

            if (!ModelState.IsValid)
            {
                return View("Profile", memberProfileVM);
            }

            if (appUser.NormalizedUserName != member.UserName.ToUpper() && await _userManager.Users.AnyAsync(u => u.NormalizedUserName == member.UserName.ToUpper()))
            {
                ModelState.AddModelError("UserName", "UserName Alreade Exist");

                return View("Profile", memberProfileVM);
            }

            if (appUser.NormalizedEmail != member.Email.ToUpper() && await _userManager.Users.AnyAsync(u => u.NormalizedEmail == member.Email.ToUpper()))
            {
                ModelState.AddModelError("Email", "Email Alreade Exist");
                return View("Profile", memberProfileVM);
            }

            appUser.FullName = member.FullName;
            appUser.UserName = member.UserName;
            appUser.Email = member.Email;
            appUser.Address = member.Address;
            appUser.Country = member.Country;
            appUser.City = member.City;
            appUser.State = member.State;
            appUser.ZipCode = member.ZipCode;
            appUser.PhoneNumber = member.PhoneNumber;

            IdentityResult identityResult = await _userManager.UpdateAsync(appUser);

            if (!identityResult.Succeeded)
            {
                foreach (var item in identityResult.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View("Profile", memberProfileVM);
            }

            if (member.Password != null)
            {
                if (string.IsNullOrWhiteSpace(member.CurrentPassword))
                {
                    ModelState.AddModelError("CurrentPassword", "CurrentPassword Is Requered");
                    return View("Profile", memberProfileVM);
                }

                if (!await _userManager.CheckPasswordAsync(appUser, member.CurrentPassword))
                {
                    ModelState.AddModelError("CurrentPassword", "CurrentPassword Is InCorrect");
                    return View("Profile", memberProfileVM);
                }

                string token = await _userManager.GeneratePasswordResetTokenAsync(appUser);
                identityResult = await _userManager.ResetPasswordAsync(appUser, token, member.Password);
                if (!identityResult.Succeeded)
                {
                    foreach (var item in identityResult.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                    return View("Profile", memberProfileVM);
                }
            }

            return RedirectToAction("Profile");
        }

        #region Create Role
        //public async Task<IActionResult> CreateRole()
        //{
        //    await _roleManager.CreateAsync(new IdentityRole { Name = "SuperAdmin" });
        //    await _roleManager.CreateAsync(new IdentityRole { Name = "Admin" });
        //    await _roleManager.CreateAsync(new IdentityRole { Name = "Member" });

        //    return Ok();
        //}
        #endregion
    }
}
