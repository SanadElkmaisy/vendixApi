using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using VendixPos.Models;

namespace VendixPos.DTOs
{
    public class CreateItemDto
    {
        [Required]
        public int DepartmentID { get; set; }

        [Required]
        public int ItemNum { get; set; }

        [Required]
        [StringLength(200)]
        public string ItemName { get; set; }

        public int Ranking { get; set; }

        [Range(0, int.MaxValue)]
        public int? ItemQuantity { get; set; }

        public bool? ItemInactive { get; set; }

        [StringLength(50)]
        public string ItemColor { get; set; }

        public bool? ItemNoQuan { get; set; }

        public int? LowestItemQun { get; set; }

        public IFormFile? ItemImage { get; set; }

        // Barcode information
        [Required]
        public string BarcodeValue { get; set; }



        // IMPORTANT: bind Units as JSON string
        public ICollection<CreateItemUnitDto> Units { get; set; } = new List<CreateItemUnitDto>();
        [Required]
        public string UnitsJson { get; set; }
    }

    public class CreateItemUnitDto
    {
        [Required]
        public string SecondUnit { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public int UnitQuantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public double UnitPrice { get; set; }

        public double? LowPrice { get; set; }

        public bool Checked { get; set; } = true;
    }

    public class ItemResponseDto
    {
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public string BarcodeValue { get; set; }
        public string Message { get; set; }
    }
}