namespace VendixPos.DTOs
{
    public class InventoryTvp
    {
        public int InventoryAddID { get; set; }
        public int InventoryState { get; set; }
        public decimal InventoryInvSUQ { get; set; }
        public int InventoryItemNum { get; set; }
        public int InventoryInvoNum { get; set; }
        public int SupplierID { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime InsertedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int InsertedBy { get; set; }
        public int UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
    public class InventoryMovementDto
    {
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public string MovementType { get; set; } // "IN", "OUT", "ADJUSTMENT"
        public DateTime MovementDate { get; set; }
        public int? SupplierID { get; set; }
        public string Reference { get; set; } // Invoice/Sell number
    }

    public class InventoryLevelDto
    {
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public int CurrentStock { get; set; }
        public int ReorderLevel { get; set; }
    }
}
