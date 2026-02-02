using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIPM_Backend.Models
{
    /// <summary>
    /// Operatorët Ekonomikë / Distributorët
    /// </summary>
    [Table("Distributor")]
    public class Distributor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Emri { get; set; } = string.Empty;

        [StringLength(50)]
        public string? NIPT { get; set; }

        [StringLength(500)]
        public string? Adresa { get; set; }

        [StringLength(50)]
        public string? NumriTelefonit { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(100)]
        public string? Website { get; set; }

        [StringLength(200)]
        public string? PersoniKontaktues { get; set; }

        public bool EshteAktiv { get; set; } = true;

        public DateTime DataRegjistrimit { get; set; } = DateTime.Now;

        [StringLength(1000)]
        public string? Shënime { get; set; }

        // Child table - Inxhinierët e distributor-it
        public virtual ICollection<DistributorInxhinier>? Inxhinierët { get; set; }
    }

    /// <summary>
    /// Inxhinierët e Distributor-it (Child Table)
    /// </summary>
    [Table("DistributorInxhinier")]
    public class DistributorInxhinier
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int DistributorId { get; set; }

        [ForeignKey("DistributorId")]
        public virtual Distributor? Distributor { get; set; }

        [Required]
        [StringLength(200)]
        public string Emri { get; set; } = string.Empty;

        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(50)]
        public string? Telefoni { get; set; }

        [StringLength(100)]
        public string? Pozicioni { get; set; }

        public bool EshteKontaktiKryesor { get; set; } = false;

        public bool Pranojnjoftime { get; set; } = true;
    }
}
