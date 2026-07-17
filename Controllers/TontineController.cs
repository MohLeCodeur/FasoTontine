using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using FasoTontine.Models;

namespace FasoTontine.Controllers
{
    public class TontineController : Controller
    {
        public IActionResult Index()
        {
            // Return empty list of tontines for design view
            var tontines = new List<Tontine>();
            return View(tontines);
        }

        public IActionResult Details(int id)
        {
            // Stub details data to prevent NullReferenceExceptions in details view rendering
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
            // TODO: Implement create database logic here
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Join(int id)
        {
            // TODO: Implement join tontine logic here
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GenerateDrawOrder(int id, string method)
        {
            // TODO: Implement draw order logic here
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
