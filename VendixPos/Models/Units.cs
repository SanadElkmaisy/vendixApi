using System.ComponentModel.DataAnnotations;

namespace VendixPos.Models
{
    public class Units
    {
        [Key]
        public int UnitID { get; set; }
        public int ItemID { get; set; }
        public string SecondUnit { get; set; }
        public int UnitQuantity { get; set; }
        public double UnitPrice { get; set; }
        public bool Checked { get; set; }
        public DateTime InsertedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int InsertedBy { get; set; }
        public int UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public double LowPrice { get; set; }

    }
}
