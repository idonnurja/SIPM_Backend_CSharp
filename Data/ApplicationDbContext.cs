using Microsoft.EntityFrameworkCore;
using SIPM_Backend.Models;

namespace SIPM_Backend.Data
{
    /// <summary>
    /// Database Context për Entity Framework Core
    /// Përdor Code-First Approach
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet - Tabelat në database
        public DbSet<Pajisje> Pajisje { get; set; }
        public DbSet<AktKonstatimi> AktKonstatimi { get; set; }
        public DbSet<Nderhyrje> Nderhyrje { get; set; }
        public DbSet<Distributor> Distributor { get; set; }
        public DbSet<DistributorInxhinier> DistributorInxhinier { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===== KONFIGURIMI I RELACIONEVE =====

            // Pajisje -> AktKonstatimi (One-to-Many)
            modelBuilder.Entity<AktKonstatimi>()
                .HasOne(a => a.Pajisje)
                .WithMany(p => p.AkteKonstatimit)
                .HasForeignKey(a => a.PajisjeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Pajisje -> Nderhyrje (One-to-Many)
            modelBuilder.Entity<Nderhyrje>()
                .HasOne(n => n.Pajisje)
                .WithMany(p => p.Nderhyrjet)
                .HasForeignKey(n => n.PajisjeId)
                .OnDelete(DeleteBehavior.Restrict);

            // AktKonstatimi -> Nderhyrje (One-to-One optional)
            modelBuilder.Entity<Nderhyrje>()
                .HasOne(n => n.AktKonstatimi)
                .WithOne(a => a.Nderhyrja)
                .HasForeignKey<Nderhyrje>(n => n.AktKonstatimiId)
                .OnDelete(DeleteBehavior.SetNull);

            // Distributor -> DistributorInxhinier (One-to-Many)
            modelBuilder.Entity<DistributorInxhinier>()
                .HasOne(di => di.Distributor)
                .WithMany(d => d.Inxhinierët)
                .HasForeignKey(di => di.DistributorId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===== INDEKSET PËR PERFORMANCE =====
            modelBuilder.Entity<Pajisje>()
                .HasIndex(p => p.DeviceId)
                .IsUnique();

            modelBuilder.Entity<Pajisje>()
                .HasIndex(p => p.StatusiTeknik);

            modelBuilder.Entity<AktKonstatimi>()
                .HasIndex(a => a.Statusi);

            modelBuilder.Entity<Nderhyrje>()
                .HasIndex(n => n.Statusi);

            // ===== DEFAULT VALUES =====
            modelBuilder.Entity<Pajisje>()
                .Property(p => p.DataKrijimit)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<AktKonstatimi>()
                .Property(a => a.DataKrijimit)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Nderhyrje>()
                .Property(n => n.DataHapjes)
                .HasDefaultValueSql("GETDATE()");

            // ===== SEED DATA (Të dhëna fillestare për testim) =====
            SeedData(modelBuilder);
        }

        /// <summary>
        /// Të dhëna fillestare për testim
        /// </summary>
        private void SeedData(ModelBuilder modelBuilder)
        {
            // Pajisje Fillestare
            modelBuilder.Entity<Pajisje>().HasData(
                new Pajisje
                {
                    Id = 1,
                    DeviceId = "QSUT-EKG-6500-001",
                    Emri = "Elektrokardiograf GE MAC 5500",
                    Kategoria = "Diagnostikë Kardiologjike",
                    Prodhues = "GE Healthcare",
                    Modeli = "MAC 5500",
                    NumriSerial = "SN-GE-001-2024",
                    VleraBlerjes = 12500.00M,
                    DataBlerjes = new DateTime(2024, 1, 15),
                    DataFillimitPerdorimit = new DateTime(2024, 2, 1),
                    Vendndodhja = "Kardiologji - Dhoma 302",
                    Sherbimi = "Kardiologji",
                    Godina = "Godina Kryesore",
                    StatusiTeknik = "Aktive",
                    DataKrijimit = DateTime.Now,
                    EshteAktive = true,
                    ViteJetese = 10
                },
                new Pajisje
                {
                    Id = 2,
                    DeviceId = "QSUT-XRY-8800-002",
                    Emri = "Aparat Rreze-X Mobil",
                    Kategoria = "Imazheri Radiologjike",
                    Prodhues = "Siemens Healthineers",
                    Modeli = "Mobilett Mira",
                    NumriSerial = "SN-SIE-002-2024",
                    VleraBlerjes = 45000.00M,
                    DataBlerjes = new DateTime(2023, 11, 20),
                    DataFillimitPerdorimit = new DateTime(2023, 12, 5),
                    Vendndodhja = "Radiologji - Njësia Mobile",
                    Sherbimi = "Radiologji",
                    Godina = "Godina Kryesore",
                    StatusiTeknik = "Aktive",
                    DataKrijimit = DateTime.Now,
                    EshteAktive = true,
                    ViteJetese = 12
                },
                new Pajisje
                {
                    Id = 3,
                    DeviceId = "QSUT-VNT-2100-003",
                    Emri = "Ventilator Intensiv",
                    Kategoria = "Suport Jetësor",
                    Prodhues = "Dräger",
                    Modeli = "Evita V300",
                    NumriSerial = "SN-DRG-003-2024",
                    VleraBlerjes = 28000.00M,
                    DataBlerjes = new DateTime(2024, 3, 10),
                    DataFillimitPerdorimit = new DateTime(2024, 3, 25),
                    Vendndodhja = "ICU - Dhoma 105",
                    Sherbimi = "Terapia Intensive",
                    Godina = "Godina Kryesore",
                    StatusiTeknik = "Aktive",
                    DataKrijimit = DateTime.Now,
                    EshteAktive = true,
                    ViteJetese = 10
                }
            );

            // Distributor Fillestar
            modelBuilder.Entity<Distributor>().HasData(
                new Distributor
                {
                    Id = 1,
                    Emri = "Med-Tech Solutions Albania",
                    NIPT = "K12345678L",
                    Adresa = "Rruga Qemal Stafa, Tiranë",
                    NumriTelefonit = "+355 4 2234567",
                    Email = "info@medtech.al",
                    Website = "www.medtech.al",
                    PersoniKontaktues = "Arben Hoxha",
                    EshteAktiv = true,
                    DataRegjistrimit = DateTime.Now
                }
            );

            // Inxhinierë Distributor
            modelBuilder.Entity<DistributorInxhinier>().HasData(
                new DistributorInxhinier
                {
                    Id = 1,
                    DistributorId = 1,
                    Emri = "Petrit Kola",
                    Email = "petrit.kola@medtech.al",
                    Telefoni = "+355 69 1234567",
                    Pozicioni = "Senior Biomedical Engineer",
                    EshteKontaktiKryesor = true,
                    Pranojnjoftime = true
                },
                new DistributorInxhinier
                {
                    Id = 2,
                    DistributorId = 1,
                    Emri = "Elona Gjika",
                    Email = "elona.gjika@medtech.al",
                    Telefoni = "+355 69 7654321",
                    Pozicioni = "Field Service Engineer",
                    EshteKontaktiKryesor = false,
                    Pranojnjoftime = true
                }
            );
        }
    }
}
