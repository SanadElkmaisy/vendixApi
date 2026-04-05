using System.ComponentModel.DataAnnotations;

namespace VendixPos.Models
{
    public class AddInventory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Inventroy { get; set; }
    }
}