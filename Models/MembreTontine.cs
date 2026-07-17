using System;

namespace FasoTontine.Models
{
    public class MembreTontine
    {
        public int TontineId { get; set; }
        public virtual Tontine? Tontine { get; set; }

        public int UtilisateurId { get; set; }
        public virtual User? Utilisateur { get; set; }

        public int OrdreTirage { get; set; } // Position dans le calendrier des tirages/tours
        public DateTime DateIntegration { get; set; } = DateTime.Now;
        public string Statut { get; set; } = "Actif"; // "Actif", "Suspendu"
    }
}
