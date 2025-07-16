using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace VendixPos.Models
{
    public class Item
    {
        [Key]
        public int ItemID { get; set; }

        [Required]
        public int DepartmentID { get; set; }

        [Required]
        public int ItemNum { get; set; }

        [Required]
        [StringLength(100)]
        public string ItemName { get; set; }

        [Required]
        public int Ranking { get; set; }

        public int? ItemQuantity { get; set; }
        public bool ItemInactive { get; set; }
        public bool? ItemQuickList { get; set; }
        public bool? ItemNoQuan { get; set; }
        public int? InsertedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? InsertedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsDeleted { get; set; }

        [Column(TypeName = "image")]
        public byte[]? Pic { get; set; }

        [StringLength(50)]
        public string ItemColor { get; set; }

        public decimal? AccType { get; set; }
                                                                                
    }
}
