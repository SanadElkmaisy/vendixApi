using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VendixPos.Models
{
    [Table("GroupPermissionSettings")]
    public class GroupPermissionSetting
    {
        [Key]
        [Column(Order = 0)]
        public int GroupId { get; set; }

        [Key]
        [Column(Order = 1)]
        public int PermissionId { get; set; }

        [StringLength(50)]
        public string SettingValue { get; set; }

        // Navigation property
        [ForeignKey("GroupId,PermissionId")]
        public virtual GroupPermission GroupPermission { get; set; }
    }
}
