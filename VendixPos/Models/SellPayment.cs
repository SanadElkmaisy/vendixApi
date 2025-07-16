namespace VendixPos.Models
{
    public class SellPayment
    {
        public int SellFinancialID { get; set; }
        public int ShiftID { get; set; }
        public decimal SellPaymentValue { get; set; }
        public DateTime InsertDate { get; set; }
        public int UserID { get; set; }
        public int? SupplierId { get; set; }
    }
}
