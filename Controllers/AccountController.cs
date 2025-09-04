using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMS.Data.Entities;
using RMS.Data.Repository;
using RMS.Data.Services;
using RMS.Web.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RMS.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly DataContext _context;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly IUserService userService;

        public AccountController(DataContext context, IUserService userService)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
            this.userService = userService;
        }

        // ================= LOGIN =================
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserLoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid email or password");
                return View(model);
            }

            var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.Password, model.Password);
            if (verifyResult == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("", "Invalid email or password");
                return View(model);
            }

            // Create claims including role
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Home");
        }

        // ================= REGISTER =================
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existingUser = await _context.Users.AnyAsync(u => u.Email == model.Email);
            if (existingUser)
            {
                ModelState.AddModelError("Email", "Email is already taken");
                return View(model);
            }

            model.Password = _passwordHasher.HashPassword(model, model.Password);
            model.Role = Role.user;
            model.UserName = model.Name;

            _context.Users.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }

        // ================= LOGOUT =================
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // ================= FORGOT PASSWORD =================
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(string email)
        {
            var token = userService.GeneratePasswordResetToken(email);
            if (token == null)
            {
                ViewBag.Error = "No user found with that email.";
                return View();
            }

            // In production, send email with reset link
            ViewBag.Message = $"Password reset link: /Account/ResetPassword?token={token}";
            return View();
        }

        // ================= RESET PASSWORD =================
        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            ViewBag.Token = token;
            return View();
        }

        [HttpPost]
        public IActionResult ResetPassword(string token, string newPassword)
        {
            var success = userService.ResetPassword(token, newPassword);
            if (!success)
            {
                ViewBag.Error = "Invalid or expired reset token.";
                ViewBag.Token = token;
                return View();
            }

            return RedirectToAction("Login");
        }
    }
}
