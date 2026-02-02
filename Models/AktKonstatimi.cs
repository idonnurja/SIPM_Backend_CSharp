using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIPM_Backend.Models
{
    /// <summary>
    /// Akt Konstatimi i dëmtimit - krijohet nga Teknikët
    /// </summary>
    [Table("AktKonstatimi")]
    public class AktKonstatimi
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Foreign Key - lidhja me Pajisjen
        [Required]
        public int PajisjeId { get; set; }

        [ForeignKey("PajisjeId")]
        public virtual Pajisje? Pajisje { get; set; }

        [Required]
        [StringLength(1000)]
        public string Pershkrimi { get; set; } = string.Empty;

        [StringLength(20)]
        [Required]
        public string Statusi { get; set; } = "HAPUR";
        // Vlerat: "HAPUR", "MBYLLUR"

        [StringLength(100)]
        [Required]
        public string KrijuarNga { get; set; } = string.Empty;
        // Emri i teknikut që krijoi aktin

        public DateTime DataKrijimit { get; set; } = DateTime.Now;

        public DateTime? DataMbylljes { get; set; }

        [StringLength(100)]
        public string? MbyllurNga { get; set; }
        // Emri i inxhinierit që e mbyllur

        [StringLength(2000)]
        public string? NotaMbylljes { get; set; }
        // Pershkrimi i riparimit të kryer

        [StringLength(50)]
        public string? NiveliUrgjences { get; set; }
        // "I lartë", "Mesatar", "I ulët"

        // Navigation Property për Ndërhyrjen që lidhet me këtë akt
        public virtual Nderhyrje? Nderhyrja { get; set; }
    }
}
