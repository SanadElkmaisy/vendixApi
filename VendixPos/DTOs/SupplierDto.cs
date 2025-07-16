using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VendixPos.DTOs
{
    public class SupplierDto
    {
        public int id { get; set; }
        public string supplierId { get; set; }
        public string SupplierName { get; set; }
        public string SupplierAddress { get; set; }
        public string SupplierNumber { get; set; }
        public string SupplierEmail { get; set; }
        public bool StateSup { get; set; }
        public int SupplierType { get; set; }
        public int FileCSID { get; set; }
        public double StartBalance { get; set; }
        public DateTime InsertedDate { get; set; }

    }
}
