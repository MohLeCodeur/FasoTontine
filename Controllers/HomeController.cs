using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FasoTontine.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FasoTontine.Models;

namespace FasoTontine.Controllers
{
    public class HomeController : Controller
    {
        private readonly FasoTontineContext _db;

        public HomeController(FasoTontineContext db)
        {
            _db = db;
        }

        private int CurrentUserId => int.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var id) ? id : 2;

        public async Task<IActionResult> Index()
        {
            var participations = await _db.MembresTontines
                .Where(m => m.UtilisateurId == CurrentUserId)
                .Include(m => m.Tontine)
                .OrderBy(m => m.Tontine!.Statut)
                .ToListAsync();
            var tontineIds = participations.Select(m => m.TontineId).ToList();
            var payments = await _db.Cotisations
                .Where(c => c.CotiseurId == CurrentUserId && tontineIds.Contains(c.TontineId))
                .ToListAsync();
            var pending = await _db.Cotisations
                .CountAsync(c => tontineIds.Contains(c.TontineId) && c.Statut == "EnAttente");

            ViewBag.MyParticipations = participations;
            ViewBag.PendingValidationsCount = pending;
            ViewBag.TotalSaved = payments.Where(c => c.Statut == "Valide").Sum(c => c.MontantPaye);
            ViewBag.NextPaymentsCount = participations.Count(m => m.Tontine?.Statut == "Active" &&
                !payments.Any(c => c.TontineId == m.TontineId && c.NumeroTour == m.OrdreTirage && c.Statut == "Valide"));
            ViewBag.Notifications = await _db.Notifications
                .Where(n => n.UtilisateurId == CurrentUserId)
                .OrderByDescending(n => n.DateCreation)
                .Take(3)
                .ToListAsync();

            return View();
        }

        public async Task<IActionResult> Notifications()
        {
            var notifications = await _db.Notifications
                .Where(n => n.UtilisateurId == CurrentUserId)
                .OrderByDescending(n => n.DateCreation)
                .ToListAsync();
            return View(notifications);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkNotificationsRead()
        {
            var notifications = await _db.Notifications.Where(n => n.UtilisateurId == CurrentUserId && !n.EstLu).ToListAsync();
            notifications.ForEach(n => n.EstLu = true);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Notifications));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
