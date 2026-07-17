using System;
using System.Collections.Generic;

namespace FasoTontine.Models
{
    public class Tontine
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal MontantPart { get; set; }
        public string Frequence { get; set; } = "Mensuelle"; // "Hebdomadaire", "Mensuelle"
        public DateTime DateDebut { get; set; } = DateTime.Today;
        public DateTime DateFin { get; set; } = DateTime.Today.AddMonths(6);
        public string Statut { get; set; } = "EnAttente"; // "EnAttente", "Active", "Terminee"
        public int CreeParUserId { get; set; }
        
        // Navigation-like helpers
        public virtual User? CreePar { get; set; }
        public List<MembreTontine> Membres { get; set; } = new List<MembreTontine>();
        public List<Cotisation> Cotisations { get; set; } = new List<Cotisation>();
        public List<Versement> Versements { get; set; } = new List<Versement>();
    }
}
