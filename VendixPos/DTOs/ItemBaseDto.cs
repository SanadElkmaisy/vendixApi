namespace VendixPos.DTOs
{
    public class ItemBaseDto
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public double UnitPrice { get; set; }
        public int? ItemQuantity { get; set; }
        public bool ItemNoQuantity { get; set; }
        public byte[] Pic { get; set; }
    }
}
