namespace VendixPos.Models
{
    public class Items
    {
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public decimal UnitPrice { get; set; }
        public byte[] Pic { get; set; }

    }
}
