using System.ComponentModel.DataAnnotations.Schema;

namespace VendixPos.Models
{
    [Table("PaymentMothedTable")]
    public class PaymentMethod
    {
        [Column("PaymentMothetId")]
        public int PaymentMethodId { get; set; }

        [Column("PaymentmothetName")]
        public string PaymentmothetName { get; set; }
    }
}