using System.ComponentModel.DataAnnotations;

namespace VendixPos.DTOs
{
    // Request DTO (for API input)
    public class SellInfoRequestDto
    {
        [Required]
        public int Customer { get; set; }

        [Required]
        public int PayMethod { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TotalValue { get; set; }

        public decimal Sales { get; set; }
        public decimal TotalAll { get; set; }
        public int State { get; set; }
        public bool Delivered { get; set; }

        // Client shouldn't set these - will be set by server
        //public int InsertedBy => CurrentUser.Id; // Example
        public int InsertedBy => 1; // Example
        public DateTime InsertedDate => DateTime.UtcNow;
    }

    // Response DTO (for API output)
    public class SellInfoResponseDto
    {
        public int SellID { get; set; }
        public string InvoiceNumber { get; set; }
        public int Customer { get; set; }
        public int PayMethod { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime SellDate { get; set; }
        public bool Delivered { get; set; }
        public string Status { get; set; } // Could be enum
    }
}
