namespace VendixPos.Models
{
    public class SellInfo
    {
        public int Customer { get; set; }
        public int PayMethod { get; set; }
        public decimal TotalValue { get; set; }
        public decimal Sales { get; set; }
        public decimal TotalAll { get; set; }
        public int State { get; set; }
        public int InsertedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime InsertedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int Status { get; set; }
        public bool Delivered { get; set; }
    }
}
