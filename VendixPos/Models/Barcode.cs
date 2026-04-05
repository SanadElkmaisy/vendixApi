using System.ComponentModel.DataAnnotations;

namespace VendixPos.Models
{
    public class Barcode
    {
        [Key]
        public int BarcodeID { get; set; }
        public int ItemId { get; set; }
        public Item Item { get; set; }
        public string BarcodeValue { get; set; }

    }
}
