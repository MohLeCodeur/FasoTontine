using System;

namespace FasoTontine.Models
{
    public class Versement
    {
        public int Id { get; set; }
        public int TontineId { get; set; }
        public virtual Tontine? Tontine { get; set; }
        public int BeneficiaireId { get; set; }
        public virtual User? Beneficiaire { get; set; }
        public int NumeroTour { get; set; }
        public decimal MontantVerse { get; set; }
        public DateTime DateVersement { get; set; }
        public string Statut { get; set; } = "EnAttente";
    }
}
