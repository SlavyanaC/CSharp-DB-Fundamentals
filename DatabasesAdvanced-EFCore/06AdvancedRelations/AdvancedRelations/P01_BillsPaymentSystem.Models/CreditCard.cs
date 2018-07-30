namespace P01_BillsPaymentSystem.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class CreditCard
    {
        [Key]
        public int CreditCardId { get; set; }

        [Required]
        public decimal Limit { get; set; }

        [Required]
        public decimal MoneyOwed { get; set; }

        [NotMapped]
        public decimal LimitLeft => this.Limit - this.MoneyOwed;

        [Required]
        public DateTime ExpirationDate { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public void Deposit(decimal amount)
        {
            if (amount > 0)
            {
                this.MoneyOwed -= amount;
            }
        }

        public void Withdraw(decimal amount)
        {
            if (LimitLeft - amount >= 0)
            {
                this.MoneyOwed += amount;
            }
        }
    }
}
