using System.ComponentModel.DataAnnotations;

namespace VendixPos.Models
{
    public class FastGroupWebPos
    {
        [Key]
        public int FastItemGroupID { get; set; }
        [Required(ErrorMessage = "Group name is required")]
        public string FastItemGroupName { get; set; }
    }
}
