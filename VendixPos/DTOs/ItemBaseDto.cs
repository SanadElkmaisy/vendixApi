namespace VendixPos.DTOs
{
    public class ItemBaseDto
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public double UnitPrice { get; set; }
        public int? ItemQuantity { get; set; }
        public bool ItemNoQuan { get; set; }
        public byte[] Pic { get; set; }
    }


    public class ItemUnitDto
    {
        public string SecondUnit { get; set; }
        public int UnitQuantity { get; set; }
        public double UnitPrice { get; set; }
        public double? LowPrice { get; set; }
    }
}
