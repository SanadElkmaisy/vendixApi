using System.ComponentModel.DataAnnotations;

namespace VendixPos.Models
{
    public class FastITemsWebPos
    {
        [Key]
        public int Id { get; set; }
        public int GroupId { get; set; } 
        public int ItemId { get; set; }
      
    }
}
