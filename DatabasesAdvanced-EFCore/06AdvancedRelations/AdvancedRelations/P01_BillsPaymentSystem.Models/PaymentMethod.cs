namespace P01_BillsPaymentSystem.Data.Models
{
    using P01_BillsPaymentSystem.Data.Models.Attributes;
    using P01_BillsPaymentSystem.Data.Models.Enums;
    using System.ComponentModel.DataAnnotations;

    public class PaymentMethod
    {
        [Key]
        public int Id  { get; set; }

        [Required]
        public PaymentMethodType Type { get; set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; }
        
        [Xor(nameof(CreditCardId))]
        public int? BankAccountId { get; set; }
        public BankAccount BankAccount { get; set; }

        public int? CreditCardId { get; set; }
        public CreditCard CreditCard { get; set; }
    }
}
