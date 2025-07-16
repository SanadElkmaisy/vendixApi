using System.ComponentModel.DataAnnotations;

namespace VendixPos.DTOs
{
    public class PaymentInfoDto
    {
        public string PaymentMethod { get; set; } // "Cash", "Credit", "Transfer"

        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        public string TransactionReference { get; set; }
        public DateTime PaymentDate { get; set; }
    }

    public class PaymentResultDto
    {
        public bool Success { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal BalanceDue { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
