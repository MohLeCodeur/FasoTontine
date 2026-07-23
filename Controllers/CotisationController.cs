using System;
using System.Linq;
using System.Threading.Tasks;
using FasoTontine.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FasoTontine.Models;

namespace FasoTontine.Controllers
{
    public class CotisationController : Controller
    {
        private readonly FasoTontineContext _db;

        public CotisationController(FasoTontineContext db)
        {
            _db = db;
        }

        private int CurrentUserId => int.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var id) ? id : 2;

        [HttpGet]
        public async Task<IActionResult> Declarer(int tontineId, int tour)
        {
            var tontine = await _db.Tontines.FindAsync(tontineId);
            var member = await _db.MembresTontines.FindAsync(tontineId, CurrentUserId);
            if (tontine == null || member == null || tontine.Statut != "Active") return NotFound();

            var model = new Cotisation
            {
                TontineId = tontineId,
                CotiseurId = CurrentUserId,
                NumeroTour = tour,
                MontantPaye = tontine.MontantPart,
                DatePaiement = DateTime.Now
            };

            ViewBag.TontineNom = tontine.Nom;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Declarer(Cotisation model)
        {
            var tontine = await _db.Tontines.FindAsync(model.TontineId);
            var member = await _db.MembresTontines.FindAsync(model.TontineId, CurrentUserId);
            if (tontine == null || member == null || tontine.Statut != "Active") return NotFound();
            if (model.NumeroTour < 1 || model.NumeroTour > await _db.MembresTontines.CountAsync(m => m.TontineId == model.TontineId && m.Statut == "Actif"))
                return BadRequest("Tour invalide.");
            if (string.IsNullOrWhiteSpace(model.TransactionReference))
                ModelState.AddModelError(nameof(model.TransactionReference), "La référence est obligatoire.");
            if (!ModelState.IsValid) return View(model);

            var alreadyDeclared = await _db.Cotisations.AnyAsync(c => c.TontineId == model.TontineId && c.CotiseurId == CurrentUserId &&
                c.NumeroTour == model.NumeroTour && c.Statut != "Rejete");
            if (alreadyDeclared) return Conflict("Ce tour a déjà été déclaré.");

            model.CotiseurId = CurrentUserId;
            model.MontantPaye = tontine.MontantPart;
            model.DatePaiement = DateTime.Now;
            model.Statut = "EnAttente";
            _db.Cotisations.Add(model);
            _db.Notifications.Add(new Notification
            {
                UtilisateurId = tontine.CreeParUserId,
                Message = $"Un paiement de {model.MontantPaye:N0} FCFA a été déclaré pour le tour #{model.NumeroTour} de la tontine '{tontine.Nom}'."
            });
            await _db.SaveChangesAsync();
            return RedirectToAction("Details", "Tontine", new { id = model.TontineId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Valider(int id, string decision)
        {
            var payment = await _db.Cotisations.Include(c => c.Tontine).FirstOrDefaultAsync(c => c.Id == id);
            if (payment == null) return NotFound();
            var admin = await _db.Users.FindAsync(CurrentUserId);
            if (admin == null || (admin.Role != "Admin" && admin.Role != "SuperAdmin")) return Forbid();

            var valid = string.Equals(decision, "valider", StringComparison.OrdinalIgnoreCase);
            payment.Statut = valid ? "Valide" : "Rejete";
            _db.Notifications.Add(new Notification
            {
                UtilisateurId = payment.CotiseurId,
                Message = valid
                    ? $"Votre paiement de {payment.MontantPaye:N0} FCFA pour le tour #{payment.NumeroTour} de la tontine '{payment.Tontine?.Nom}' a été validé."
                    : $"Votre paiement pour le tour #{payment.NumeroTour} de la tontine '{payment.Tontine?.Nom}' a été rejeté. Vérifiez votre référence."
            });
            await _db.SaveChangesAsync();
            return RedirectToAction("Details", "Tontine", new { id = payment.TontineId });
        }
    }
}
