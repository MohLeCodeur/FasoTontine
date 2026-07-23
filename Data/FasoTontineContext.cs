using FasoTontine.Models;
using Microsoft.EntityFrameworkCore;

namespace FasoTontine.Data;

public class FasoTontineContext : DbContext
{
    public FasoTontineContext(DbContextOptions<FasoTontineContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Tontine> Tontines => Set<Tontine>();
    public DbSet<MembreTontine> MembresTontines => Set<MembreTontine>();
    public DbSet<Cotisation> Cotisations => Set<Cotisation>();
    public DbSet<Versement> Versements => Set<Versement>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("users");
        modelBuilder.Entity<Tontine>().ToTable("tontines");
        modelBuilder.Entity<MembreTontine>().ToTable("membrestontines");
        modelBuilder.Entity<Cotisation>().ToTable("cotisations");
        modelBuilder.Entity<Versement>().ToTable("versements");
        modelBuilder.Entity<Notification>().ToTable("notifications");

        modelBuilder.Entity<MembreTontine>().HasKey(m => new { m.TontineId, m.UtilisateurId });
        modelBuilder.Entity<User>().Property(u => u.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<Tontine>().Property(t => t.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<Cotisation>().Property(c => c.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<Versement>().Property(v => v.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<Notification>().Property(n => n.Id).ValueGeneratedOnAdd();

        modelBuilder.Entity<Tontine>()
            .HasOne(t => t.CreePar)
            .WithMany()
            .HasForeignKey(t => t.CreeParUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MembreTontine>()
            .HasOne(m => m.Tontine)
            .WithMany(t => t.Membres)
            .HasForeignKey(m => m.TontineId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MembreTontine>()
            .HasOne(m => m.Utilisateur)
            .WithMany()
            .HasForeignKey(m => m.UtilisateurId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Cotisation>()
            .HasOne(c => c.Tontine)
            .WithMany(t => t.Cotisations)
            .HasForeignKey(c => c.TontineId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Cotisation>()
            .HasOne(c => c.Cotiseur)
            .WithMany()
            .HasForeignKey(c => c.CotiseurId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Versement>()
            .HasOne(v => v.Tontine)
            .WithMany(t => t.Versements)
            .HasForeignKey(v => v.TontineId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Versement>()
            .HasOne(v => v.Beneficiaire)
            .WithMany()
            .HasForeignKey(v => v.BeneficiaireId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.Utilisateur)
            .WithMany()
            .HasForeignKey(n => n.UtilisateurId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
