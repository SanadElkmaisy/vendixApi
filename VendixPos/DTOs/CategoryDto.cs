namespace VendixPos.DTOs
{
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Color { get; set; } // For your yellow circle styling
        public List<ItemDto> Products { get; set; } = new();
    }

}
