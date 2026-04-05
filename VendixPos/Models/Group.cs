using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VendixPos.Models
{
    public class Group
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GroupId { get; set; }

        [Required]
        [StringLength(50)]
        public string GroupName { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        public bool? IsAdmin { get; set; }

        // Navigation properties
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<GroupPermission> GroupPermissions { get; set; }
    }
}
