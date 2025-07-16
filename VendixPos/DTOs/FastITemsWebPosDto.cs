using System.ComponentModel.DataAnnotations;

namespace VendixPos.DTOs
{
    public class FastITemsWebPosDto : ItemBaseDto
    {
        [Key]
        public int Id { get; set; }
        public int GroupId { get; set; }
        public bool ItemInactive { get; internal set; }
        public string Barcode { get; set; }

        public string SecondUnit { get; set; }
        public int UnitQuantity { get; set; }


    }

}
