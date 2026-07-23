using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using FasoTontine.Models;

namespace FasoTontine.Controllers
{
    public class AuthController : Controller
    {
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
            if (string.IsNullOrWhiteSpace(telephone) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "Veuillez saisir le numéro de téléphone et le mot de passe.");
                return View();
            }

            var cleanPhone = telephone.Trim();
            string userName = "Utilisateur FasoTontine";
            string userRole = "Membre";
            string userId = "1";

            if (cleanPhone == "76000000")
            {
                userName = "Adama Diarra";
                userRole = "SuperAdmin";
                userId = "1";
            }
            else if (cleanPhone == "76000001")
            {
                userName = "Oumar Traoré";
                userRole = "Membre";
                userId = "2";
            }
            else
            {
                userName = $"Membre ({cleanPhone})";
                userRole = "Membre";
                userId = new Random().Next(10, 999).ToString();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.MobilePhone, cleanPhone),
                new Claim(ClaimTypes.Role, userRole)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            TempData["Success"] = $"Bienvenue {userName} !";
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
        public async Task<IActionResult> Register(User model, string password, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 4)
            {
                ModelState.AddModelError("", "Le mot de passe doit contenir au moins 4 caractères.");
            }

            if (password != confirmPassword)
            {
                ModelState.AddModelError("", "Les deux mots de passe ne correspondent pas.");
            }

            if (string.IsNullOrWhiteSpace(model.NomComplet))
            {
                ModelState.AddModelError("NomComplet", "Le nom complet est obligatoire.");
            }

            if (string.IsNullOrWhiteSpace(model.Telephone))
            {
                ModelState.AddModelError("Telephone", "Le numéro de téléphone est obligatoire.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var cleanPhone = model.Telephone.Trim();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, new Random().Next(10, 999).ToString()),
                new Claim(ClaimTypes.Name, model.NomComplet),
                new Claim(ClaimTypes.MobilePhone, cleanPhone),
                new Claim(ClaimTypes.Role, "Membre")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            TempData["Success"] = "Votre compte a été créé avec succès !";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["Success"] = "Vous avez été déconnecté avec succès.";
            return RedirectToAction("Login", "Auth");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            TempData["Error"] = "Accès refusé. Vous n'avez pas la permission d'accéder à cette page.";
            return RedirectToAction("Login", "Auth");
        }
    }
}
