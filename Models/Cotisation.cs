using System;

namespace FasoTontine.Models
{
    public class Cotisation
    {
        public int Id { get; set; }
        public int TontineId { get; set; }
        public virtual Tontine? Tontine { get; set; }

        public int CotiseurId { get; set; }
        public virtual User? Cotiseur { get; set; }

        public int NumeroTour { get; set; }
        public decimal MontantPaye { get; set; }
        public DateTime DatePaiement { get; set; } = DateTime.Now;
        public string MethodePaiement { get; set; } = "Orange Money"; // "Orange Money", "Moov Money", "Espèces"
        public string TransactionReference { get; set; } = string.Empty;
        public string Statut { get; set; } = "EnAttente"; // "EnAttente", "Valide", "Rejete"
    }
}
