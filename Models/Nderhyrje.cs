using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIPM_Backend.Models
{
    /// <summary>
    /// Ndërhyrja Teknike (Riparim/Mirëmbajtje)
    /// </summary>
    [Table("Nderhyrje")]
    public class Nderhyrje
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Foreign Key - Pajisja
        [Required]
        public int PajisjeId { get; set; }

        [ForeignKey("PajisjeId")]
        public virtual Pajisje? Pajisje { get; set; }

        // Foreign Key - Akti i Konstatimit (optional)
        public int? AktKonstatimiId { get; set; }

        [ForeignKey("AktKonstatimiId")]
        public virtual AktKonstatimi? AktKonstatimi { get; set; }

        [Required]
        [StringLength(200)]
        public string Titulli { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Pershkrimi { get; set; } = string.Empty;

        [StringLength(50)]
        [Required]
        public string Lloji { get; set; } = "Riparim";
        // Vlerat: "Riparim", "Mirëmbajtje Preventive", "Kalibrim", "Kolaudim"

        [StringLength(50)]
        [Required]
        public string Statusi { get; set; } = "Hapur";
        // Vlerat: "Hapur", "Në Proces", "Përfunduar", "Refuzuar"

        public DateTime DataHapjes { get; set; } = DateTime.Now;

        [Column(TypeName = "date")]
        public DateTime? DataPlanifikuar { get; set; }

        public DateTime? DataFillimit { get; set; }

        public DateTime? DataPerfundimit { get; set; }

        [StringLength(100)]
        public string? InxhinieriPergjegjës { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Kostoja { get; set; }

        [StringLength(2000)]
        public string? NotaPerfundimit { get; set; }

        [StringLength(500)]
        public string? Dokumentacioni { get; set; }
        // Path to uploaded documents

        public int? PjesëzëKëmbyera { get; set; }

        [StringLength(1000)]
        public string? MaterialetPërdorura { get; set; }

        public bool KërkonAprovim { get; set; } = false;

        [StringLength(100)]
        public string? AprovuarNga { get; set; }

        public DateTime? DataAprovimit { get; set; }
    }
}
