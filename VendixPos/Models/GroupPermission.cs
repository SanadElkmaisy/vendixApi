using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VendixPos.Models
{
    [Table("GroupPermissions")]
    public class GroupPermission
    {
        [Key]
        [Column(Order = 0)]
        public int GroupId { get; set; }

        [Key]
        [Column(Order = 1)]
        public int PermissionId { get; set; }

        // Navigation properties
        [ForeignKey("GroupId")]
        public virtual Group Group { get; set; }

        [ForeignKey("PermissionId")]
        public virtual UsersPermission UsersPermission { get; set; }

        // Navigation property for settings
        public virtual ICollection<GroupPermissionSetting> GroupPermissionSettings { get; set; }
    }
}
