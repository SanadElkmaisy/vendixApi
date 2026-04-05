namespace VendixPos.DTOs
{
    public class SalesTransactionHistoryDto
    {
        public long SellID { get; set; }
        public long InvoNumber { get; set; }
        public string PaymentMethodNames { get; set; }
        public decimal TotalAll { get; set; }
        public DateTime InsertedDate { get; set; }
        public string SupplierName { get; set; }
        public string InvoiceType { get; set; }
        public string FullName { get; set; }
        public bool Delivered { get; set; }
        public int? Customer { get; set; }
    }

    public class SalesTransactionHistoryResponseDto
    {
        public List<SalesTransactionHistoryDto> Transactions { get; set; }
        public int TotalCount { get; set; }
    }
}