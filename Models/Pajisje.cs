using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIPM_Backend.Models
{
    /// <summary>
    /// Modeli kryesor për Regjistrin e Pajisjeve Mjekësore
    /// </summary>
    [Table("Pajisje")]
    public class Pajisje
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Device ID është i detyrueshëm")]
        [StringLength(50)]
        [Column("DeviceID")]
        public string DeviceId { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Emri { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Kategoria { get; set; }

        [StringLength(100)]
        public string? Prodhues { get; set; }

        [StringLength(100)]
        public string? Modeli { get; set; }

        [StringLength(100)]
        public string? NumriSerial { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? VleraBlerjes { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DataBlerjes { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DataFillimitPerdorimit { get; set; }

        [StringLength(100)]
        public string? Vendndodhja { get; set; }

        [StringLength(100)]
        public string? Sherbimi { get; set; }

        [StringLength(100)]
        public string? Godina { get; set; }

        [StringLength(20)]
        [Required]
        public string StatusiTeknik { get; set; } = "Aktive";
        // Vlerat: "Aktive", "JoFunksionale", "JashtëPërdorimit"

        [StringLength(100)]
        public string? NumriInventarMSHMS { get; set; }

        [StringLength(500)]
        public string? Pershkrimi { get; set; }

        public DateTime DataKrijimit { get; set; } = DateTime.Now;

        public DateTime? DataPerditesimit { get; set; }

        [StringLength(100)]
        public string? PerdoruesiPergjegjës { get; set; }

        public bool EshteAktive { get; set; } = true;

        // Navigation Properties
        public virtual ICollection<AktKonstatimi>? AkteKonstatimit { get; set; }
        public virtual ICollection<Nderhyrje>? Nderhyrjet { get; set; }

        // QR Code - do gjenerohet automatikisht
        [StringLength(500)]
        public string? QRCode { get; set; }

        // Amortizimi (do llogaritet automatikisht)
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? VleraMbetur { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? AmortizimAkumuluar { get; set; }

        public int? ViteJetese { get; set; }
    }
}
