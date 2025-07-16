namespace VendixPos.DTOs
{
    public class SalesTransactionResult
    {
        public int InvoiceNumber { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public List<TransactionItemDetail> Items { get; set; } = new();
    }

}
