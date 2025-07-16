using VendixPos.Models;

namespace VendixPos.DTOs
{
    public class SellTransactionRequest
    {
        public SellInfo SellInfo { get; set; }
        public List<SellDetails> SellDetails { get; set; }
        public List<Inventory> InventoryMovements { get; set; }
        public SellPayment Payment { get; set; }
        public int UserId { get; set; }
    }
}
