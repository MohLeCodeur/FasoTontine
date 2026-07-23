using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FasoTontine.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FasoTontine.Models;

namespace FasoTontine.Controllers
{
    public class TontineController : Controller
    {
        private readonly FasoTontineContext _db;

        public TontineController(FasoTontineContext db)
        {
            _db = db;
        }

        private int CurrentUserId => int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : 2;

        public async Task<IActionResult> Index()
        {
            var tontines = await _db.Tontines.Include(t => t.Membres).OrderByDescending(t => t.DateDebut).ToListAsync();
            return View(tontines);
        }

        public async Task<IActionResult> Details(int id)
        {
            var tontine = await _db.Tontines
                .Include(t => t.CreePar)
                .Include(t => t.Membres).ThenInclude(m => m.Utilisateur)
                .Include(t => t.Cotisations).ThenInclude(c => c.Cotiseur)
                .Include(t => t.Versements).ThenInclude(v => v.Beneficiaire)
                .FirstOrDefaultAsync(t => t.Id == id);
            if (tontine == null) return NotFound();

            var currentMember = tontine.Membres.FirstOrDefault(m => m.UtilisateurId == CurrentUserId);
            var currentUser = await _db.Users.FindAsync(CurrentUserId);
            ViewBag.IsMember = currentMember?.Statut == "Actif";
            ViewBag.CurrentMember = currentMember;
            ViewBag.CurrentUserId = CurrentUserId;
            ViewBag.IsAdmin = currentUser?.Role is "Admin" or "SuperAdmin";
            ViewBag.TotalRounds = tontine.Membres.Count(m => m.Statut == "Actif");

            return View(tontine);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tontine tontine)
        {
            if (!ModelState.IsValid) return View(tontine);
            tontine.CreeParUserId = CurrentUserId;
            tontine.Statut = "EnAttente";
            _db.Tontines.Add(tontine);
            _db.MembresTontines.Add(new MembreTontine
            {
                Tontine = tontine,
                UtilisateurId = CurrentUserId,
                OrdreTirage = 0,
                Statut = "Actif"
            });
            _db.Notifications.Add(new Notification
            {
                UtilisateurId = CurrentUserId,
                Message = $"Vous avez créé la tontine '{tontine.Nom}'. Ajoutez des membres puis activez les tours."
            });
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Join(int id)
        {
            var tontine = await _db.Tontines.FindAsync(id);
            if (tontine == null) return NotFound();
            var existing = await _db.MembresTontines.FindAsync(id, CurrentUserId);
            if (existing == null)
            {
                _db.MembresTontines.Add(new MembreTontine { TontineId = id, UtilisateurId = CurrentUserId, OrdreTirage = 0, Statut = "EnAttente" });
                _db.Notifications.Add(new Notification
                {
                    UtilisateurId = tontine.CreeParUserId,
                    Message = $"Un membre a demandé à rejoindre votre tontine '{tontine.Nom}'."
                });
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveMember(int id, int utilisateurId, string decision)
        {
            var tontine = await _db.Tontines.FindAsync(id);
            var member = await _db.MembresTontines.FindAsync(id, utilisateurId);
            if (tontine == null || member == null) return NotFound();
            if (tontine.CreeParUserId != CurrentUserId) return Forbid();

            var approved = string.Equals(decision, "accepter", StringComparison.OrdinalIgnoreCase);
            member.Statut = approved ? "Actif" : "Rejete";
            if (approved) member.OrdreTirage = 0;
            _db.Notifications.Add(new Notification
            {
                UtilisateurId = utilisateurId,
                Message = approved
                    ? $"Votre adhésion à la tontine '{tontine.Nom}' a été validée."
                    : $"Votre demande d'adhésion à la tontine '{tontine.Nom}' a été rejetée."
            });
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateDrawOrder(int id, string method)
        {
            var tontine = await _db.Tontines
                .Include(t => t.Membres)
                .Include(t => t.Versements)
                .FirstOrDefaultAsync(t => t.Id == id);
            if (tontine == null) return NotFound();
            if (tontine.CreeParUserId != CurrentUserId) return Forbid();

            var members = tontine.Membres.Where(m => m.Statut == "Actif").ToList();
            if (members.Count < 2) return BadRequest("Il faut au moins deux membres actifs.");
            if (string.Equals(method, "aleatoire", StringComparison.OrdinalIgnoreCase))
                members = members.OrderBy(_ => Random.Shared.Next()).ToList();
            else
                members = members.OrderBy(m => m.DateIntegration).ThenBy(m => m.UtilisateurId).ToList();

            for (var index = 0; index < members.Count; index++) members[index].OrdreTirage = index + 1;
            tontine.Statut = "Active";
            _db.Versements.RemoveRange(tontine.Versements);
            tontine.Versements = new List<Versement>();
            foreach (var member in members)
            {
                tontine.Versements.Add(new Versement
                {
                    TontineId = id,
                    BeneficiaireId = member.UtilisateurId,
                    NumeroTour = member.OrdreTirage,
                    MontantVerse = tontine.MontantPart * members.Count,
                    DateVersement = tontine.DateDebut.AddMonths(member.OrdreTirage - 1),
                    Statut = "EnAttente"
                });
                _db.Notifications.Add(new Notification
                {
                    UtilisateurId = member.UtilisateurId,
                    Message = $"La tontine '{tontine.Nom}' est active. Votre tour de rôle est le #{member.OrdreTirage}."
                });
            }
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
