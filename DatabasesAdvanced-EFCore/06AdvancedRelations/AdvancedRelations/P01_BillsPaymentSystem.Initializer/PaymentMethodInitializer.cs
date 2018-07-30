namespace P01_BillsPaymentSystem.Initializer
{
    using P01_BillsPaymentSystem.Data.Models;
    using P01_BillsPaymentSystem.Data.Models.Enums;

    public class PaymentMethodInitializer
    {
        public static PaymentMethod[] GetPaymentMethods()
        {
            PaymentMethod[] paymentMethods = new PaymentMethod[]
            {
                new PaymentMethod() { UserId = 1, Type = PaymentMethodType.BankAccount, BankAccountId = 1},
                new PaymentMethod() { UserId = 1, Type = PaymentMethodType.CreditCard, CreditCardId = 1},
                new PaymentMethod() { UserId = 2, Type = PaymentMethodType.BankAccount, BankAccountId = 2},
                new PaymentMethod() { UserId = 2, Type = PaymentMethodType.BankAccount, CreditCardId = 2},
                new PaymentMethod() { UserId = 3, Type = PaymentMethodType.BankAccount, BankAccountId = 3},
                new PaymentMethod() { UserId = 4, Type = PaymentMethodType.CreditCard, CreditCardId = 3},
                new PaymentMethod() { UserId = 5, Type = PaymentMethodType.BankAccount, BankAccountId = 4},
            };

            return paymentMethods;
        }
    }
}
