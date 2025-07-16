using System.ComponentModel.DataAnnotations;

namespace VendixPos.DTOs
{
    public class BarcodeItemDto : ItemBaseDto
    {
        [Key]
        public int BarcodeID { get; set; }
        public string BarcodeValue { get; set; }
   

    }

}
