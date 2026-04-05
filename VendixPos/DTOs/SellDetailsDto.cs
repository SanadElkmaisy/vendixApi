using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VendixPos.DTOs
{
    public class SellDetailsDto
    {
        public int ItemID { get; set; }

        [Required]
        public string ItemName { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Quantity { get; set; }

        [Range(0, double.MaxValue)]
        public double UnitPrice { get; set; }

        public decimal? Discount { get; set; }
        public string WashType { get; set; }

        [JsonIgnore] // Calculated on server
        public decimal LineTotal => ((decimal)UnitPrice * Quantity) - (Discount ?? 0);
    }

    public class SellDetailsResponseDto
    {
        public int SellID { get; set; }
        public List<SellDetailsDto> Items { get; set; } = new();
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
    }
}
