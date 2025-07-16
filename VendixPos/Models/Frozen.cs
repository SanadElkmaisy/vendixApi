using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VendixPos.Models
{
    public class Frozen
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FrozenID { get; set; }

        [Required]
        public int ItemID { get; set; }

        [Required]
        [StringLength(50)]
        public string FrozenNumber { get; set; }

        [Required]
        public int FrozenQuantity { get; set; }

        [Required]
        [StringLength(50)]
        public string FrozenItemName { get; set; }

        [Required]
        [Column(TypeName = "float")]
        public double FrozenItemPrice { get; set; }

        [Required]
        [Column(TypeName = "float")]
        public double FrozenTotal { get; set; }

        [Required]
        [Column(TypeName = "float")]
        public double FrozenCalQuantity { get; set; }

        [StringLength(50)]
        public string? WashType { get; set; }

        public bool ItemNoQuan { get; set; }

    
    }
}