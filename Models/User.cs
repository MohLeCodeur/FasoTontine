using System;
using System.Collections.Generic;

namespace FasoTontine.Models
{
    public class User
    {
        public int Id { get; set; }
        public string NomComplet { get; set; } = string.Empty;
        public string Telephone { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string MotDePasseHash { get; set; } = string.Empty;
        public string Role { get; set; } = "Membre"; // "Admin" ou "Membre"
        public DateTime DateInscription { get; set; } = DateTime.Now;
    }
}
