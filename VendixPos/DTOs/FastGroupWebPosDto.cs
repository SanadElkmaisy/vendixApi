using System.ComponentModel.DataAnnotations;

namespace VendixPos.DTOs
{
    public class FastGroupWebPosDto
    {
        [Key]
        public int FastItemGroupID { get; set; }
        [Required(ErrorMessage = "Group name is required")]
        public string FastItemGroupName { get; set; }

    }
}
