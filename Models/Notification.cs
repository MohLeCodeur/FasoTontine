using System;

namespace FasoTontine.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int UtilisateurId { get; set; }
        public virtual User? Utilisateur { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime DateCreation { get; set; } = DateTime.Now;
        public bool EstLu { get; set; } = false;
    }
}
