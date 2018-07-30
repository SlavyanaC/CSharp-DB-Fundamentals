namespace P01_BillsPaymentSystem.Initializer
{
    using P01_BillsPaymentSystem.Data.Models;
    using System;

    public class CreditCardInitializer
    {
        public static CreditCard[] GetCreditCards()
        {
            CreditCard[] creditCards = new CreditCard[]
            {
                new CreditCard() { Limit = 1587.85m, MoneyOwed = 125.20m, ExpirationDate = DateTime.Now.AddMonths(-12) },
                new CreditCard() { Limit = 18559.25m, MoneyOwed = 1693.1m, ExpirationDate =DateTime.Now.AddMonths(-41)  },
                new CreditCard() { Limit = 1845.23m, MoneyOwed = 23.26m, ExpirationDate = DateTime.Now.AddMonths(-18) },
                new CreditCard() { Limit = 2684.36m, MoneyOwed = 2198.3m, ExpirationDate =  DateTime.Now.AddMonths(-48)},
                new CreditCard() { Limit = 4585.15m, MoneyOwed = 2187.2m, ExpirationDate = DateTime.Now.AddMonths(-83) },
                new CreditCard() { Limit = 86123.26m, MoneyOwed = 0, ExpirationDate = DateTime.Now.AddMonths(-36) },
                new CreditCard() { Limit = 35986m, MoneyOwed = 159.32m, ExpirationDate =  DateTime.Now.AddMonths(-84)},
                new CreditCard() { Limit = 12165.89m, MoneyOwed = 213.15m, ExpirationDate = DateTime.Now.AddMonths(-38) },
            };

            return creditCards;
        }
    }
}
