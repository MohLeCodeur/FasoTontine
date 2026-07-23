using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FasoTontine.Models;

namespace FasoTontine.Controllers
{
    [Authorize]
    public class CotisationController : Controller
    {
        [HttpGet]
        public IActionResult Declarer(int tontineId, int tour)
        {
            var model = new Cotisation
            {
                TontineId = tontineId,
                NumeroTour = tour,
                MontantPaye = 0,
                DatePaiement = DateTime.Now
            };

            ViewBag.TontineNom = "Tontine Template";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Declarer(Cotisation model)
        {
            // TODO : Implémenter l'enregistrement de la déclaration de transaction ici
            return RedirectToAction("Details", "Tontine", new { id = model.TontineId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Valider(int id, string decision)
        {
            // TODO : Implémenter la logique de validation des cotisations ici
            return RedirectToAction("Index", "Home");
        }
    }
}
