namespace VendixPos.Models
{
    public class SellDetails
    {
        public decimal SellInfoQuantity { get; set; }
        public string SellInfoItem { get; set; }
        public decimal SellInfoValue { get; set; }
        public decimal SellInfoTotal { get; set; }
        public int InsertedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime InserteDDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int ItemID { get; set; }
        public string WashType { get; set; }
        public int SellItemQuantity { get; set; }
        public decimal? Discount { get; set; }
        public decimal? avgitem { get; set; }
        public decimal? InvoiceItemPrice { get; set; }
    }
}
