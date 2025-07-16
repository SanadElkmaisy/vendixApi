using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VendixPos.Models
{
    public class Supplier
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string supplierId { get; set; }
        public string SupplierName { get; set; }
        public string Supplieraddreess { get; set; }
        public string SupplierNumber { get; set; }
        public string SupplierEmail { get; set; }
        public bool statesup { get; set; }
        public int suplierType { get; set; }
        public int FileCSID { get; set; }
        public double StartBalance { get; set; }
        public DateTime InsertedDate { get; set; }
    }
}
