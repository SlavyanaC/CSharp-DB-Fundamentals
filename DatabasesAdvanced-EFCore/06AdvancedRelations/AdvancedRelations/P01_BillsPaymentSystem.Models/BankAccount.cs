namespace P01_BillsPaymentSystem.Data.Models
{
    using P01_BillsPaymentSystem.Data.Models.Attributes;
    using System.ComponentModel.DataAnnotations;

    public class BankAccount
    {
        [Key]
        public int BankAccountId { get; set; }

        [Required]
        public decimal Balance { get; set; }

        [Required]
        [MaxLength(50)]
        public string BankName { get; set; }

        [Required]
        [MaxLength(20)]
        [NonUnicode]
        public string SwiftCode { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public void Deposit(decimal amount)
        {
            if (amount > 0)
            {
                this.Balance += amount;
            }
        }

        public void Withdraw(decimal amount)
        {
            if (this.Balance - amount >= 0)
            {
                this.Balance -= amount;
            }
        }
    }
}
