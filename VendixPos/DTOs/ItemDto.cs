namespace VendixPos.DTOs
{
    public class ItemDto
    {
        public int ItemID { get; set; }
        public int DepartmentID { get; set; }
        public int ItemNum { get; set; }
        public string ItemName { get; set; }
        public int Ranking { get; set; }
        public int? ItemQuantity { get; set; }
        public bool? ItemInactive { get; set; }
        public string ItemColor { get; set; }
    }
}
