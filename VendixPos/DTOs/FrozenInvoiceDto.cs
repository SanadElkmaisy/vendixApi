namespace VendixPos.DTOs
{
    public class FrozenInvoiceDto
    {
        public string FrozenNumber { get; set; }
        public List<FrozenItemDto> Items { get; set; }
        public double TotalAmount { get; set; }
    }
}
