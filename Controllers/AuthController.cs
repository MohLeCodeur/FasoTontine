using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FasoTontine.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FasoTontine.Models;

namespace FasoTontine.Controllers
{
    public class AuthController : Controller
    {
        private readonly FasoTontineContext _db;
        private readonly PasswordHasher<User> _passwordHasher = new();

        public AuthController(FasoTontineContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string telephone, string password)
        {
            var user = await _db.Users.SingleOrDefaultAsync(u => u.Telephone == telephone);
            var validPassword = user != null && (user.MotDePasseHash == password ||
                _passwordHasher.VerifyHashedPassword(user, user.MotDePasseHash, password) == PasswordVerificationResult.Success);
            if (user == null || !validPassword)
            {
                ModelState.AddModelError(string.Empty, "Téléphone ou mot de passe incorrect.");
                return View();
            }

            await SignIn(user);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User model, string password)
        {
            if (await _db.Users.AnyAsync(u => u.Telephone == model.Telephone))
                ModelState.AddModelError(nameof(model.Telephone), "Ce numéro est déjà utilisé.");
            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
                ModelState.AddModelError("password", "Le mot de passe doit contenir au moins 6 caractères.");
            if (!ModelState.IsValid) return View(model);

            model.MotDePasseHash = _passwordHasher.HashPassword(model, password);
            model.Role = "Membre";
            _db.Users.Add(model);
            await _db.SaveChangesAsync();
            await SignIn(model);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Auth");
        }

        private async Task SignIn(User user)
        {
            var claims = new[]
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id.ToString()),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.NomComplet),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.MobilePhone, user.Telephone),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, user.Role)
            };
            var identity = new System.Security.Claims.ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new System.Security.Claims.ClaimsPrincipal(identity));
        }
    }
}
