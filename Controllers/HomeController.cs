using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FasoTontine.Models;

namespace FasoTontine.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Valeurs par défaut afin que la vue du tableau de bord affiche correctement l'état initial
            ViewBag.MyParticipations = new List<MembreTontine>();
            ViewBag.PendingValidationsCount = 0;
            ViewBag.TotalSaved = 0m;
            ViewBag.NextPaymentsCount = 0;
            ViewBag.Notifications = new List<Notification>();

            return View();
        }

        public IActionResult Notifications()
        {
            // Liste vide par défaut pour la page des notifications
            var notifications = new List<Notification>();
            return View(notifications);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
