using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FasoTontine.Models;

namespace FasoTontine.Controllers
{
    [Authorize]
    public class TontineController : Controller
    {
        public IActionResult Index()
        {
            // Retourner une liste vide de tontines pour l'affichage de la vue
            var tontines = new List<Tontine>();
            return View(tontines);
        }

        public IActionResult Details(int id)
        {
            // Données factices pour éviter les erreurs de référence nulle lors de l'affichage des détails
            var tontine = new Tontine
            {
                Id = id,
                Nom = "Tontine Template",
                Description = "Description de la tontine.",
                Membres = new List<MembreTontine>(),
                Cotisations = new List<Cotisation>(),
                Versements = new List<Versement>()
            };

            ViewBag.IsMember = false;
            ViewBag.CurrentMember = null;
            ViewBag.CurrentUserId = 0;
            ViewBag.IsAdmin = true;
            ViewBag.TotalRounds = 6;

            return View(tontine);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Tontine tontine)
        {
            // TODO : Implémenter la création en base de données ici
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Join(int id)
        {
            // TODO : Implémenter la logique d'adhésion ici
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GenerateDrawOrder(int id, string method)
        {
            // TODO : Implémenter la logique de tirage au sort ici
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
