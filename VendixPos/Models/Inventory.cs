using System.ComponentModel.DataAnnotations;

namespace VendixPos.Models
{
    public class Inventory
    {
        [Key]
        public int InventoryID { get; set; }
        public int InvoiceInfoId { get; set; }
        public int InventoryAddID { get; set; }
        public byte InventoryState { get; set; }
        public int InventoryInvSUQ { get; set; }
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
}
