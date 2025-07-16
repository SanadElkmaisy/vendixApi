using System.ComponentModel.DataAnnotations;

namespace VendixPos.Models
{
    public class Invoice
    {
        [Key]
        public int InvoiceID { get; set; }
        public int InvoiceInfoId { get; set; }
        public int InvoiceItemNum { get; set; }
        public string InvoiceItemName { get; set; }
        public string InvoiceSecondUnit { get; set; }
        public int InvoiceSeUnQuan { get; set; }
        public decimal InvoicePrice { get; set; }
        public int InvoiceMainQuan { get; set; }
        public decimal InvoiceItemPrice { get; set; }
        public decimal InvoiceTotalItem { get; set; }
        public string InvoiceCurrency { get; set; }
        public decimal InvoiceRate { get; set; }
        public decimal InvoiceTotalRate { get; set; }

        public DateTime InsertedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int InsertedBy { get; set; }
        public int UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }

        public decimal InvoiceSellItemPrice { get; set; }

    }
}
