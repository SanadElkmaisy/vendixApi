namespace VendixPos.DTOs
{
    public class FrozenItemDto
    {
        public int ItemID { get; set; }
        public string FrozenItemName { get; set; }
        public double FrozenItemPrice { get; set; }
        public int FrozenQuantity { get; set; }
        public double FrozenTotal { get; set; }
        public double FrozenCalQuantity { get; set; }
        public string WashType { get; set; }
        public bool ItemNoQuan { get; set; }
    }
}
