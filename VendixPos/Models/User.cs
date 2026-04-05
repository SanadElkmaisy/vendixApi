
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VendixPos.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [StringLength(100)]
        public string Password { get; set; }

        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [ForeignKey("Group")]
        public int GroupId { get; set; }

        public bool IsActive { get; set; } = true;

        [Column("AttenticationState")]
        public bool? AuthenticationState { get; set; }

        [Column("AttenticationCode")]
        [StringLength(50)]
        public string? AuthenticationCode { get; set; }

        [Column("image")]
        public byte[] Image { get; set; }

        // Navigation properties
        public virtual Group Group { get; set; }

       
    }
}
