namespace P01_BillsPaymentSystem
{
    using System;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using P01_BillsPaymentSystem.Data;
    using P01_BillsPaymentSystem.Data.Models;
    using P01_BillsPaymentSystem.Initializer;

    class StartUp
    {
        public static void Main(string[] args)
        {
            using (BillsPaymentSystemContext context = new BillsPaymentSystemContext())
            {
                User user = GetUser(context);
                GetInfo(user);

                PayBills(user, 300);
            }
        }

        private static void PayBills(User user, decimal amount)
        {
            var bankAccountTotal = user.PaymentMethods
                .Where(p => p.BankAccount != null)
                .Sum(p => p.BankAccount.Balance);

            var creditCardsTotal = user.PaymentMethods
                .Where(p => p.CreditCard != null)
                .Sum(p => p.CreditCard.LimitLeft);

            var totalAmount = bankAccountTotal + creditCardsTotal;

            if (totalAmount >= amount)
            {
                var bankAccounts = user.PaymentMethods.Where(p => p.BankAccount != null).Select(p => p.BankAccount).OrderBy(p => p.BankAccountId).ToArray();

                foreach (var bankAccount in bankAccounts)
                {
                    if (bankAccount.Balance >= amount)
                    {
                        bankAccount.Withdraw(amount);
                        amount = 0;
                    }

                    else
                    {
                        amount -= bankAccount.Balance;
                        bankAccount.Withdraw(bankAccount.Balance);
                    }

                    if (amount == 0)
                    {
                        return;
                    }
                }

                var creaditCards = user.PaymentMethods.Where(p => p.CreditCard != null).Select(p => p.CreditCard).OrderBy(p => p.CreditCardId).ToArray();

                foreach (var creditCard in creaditCards)
                {
                    if (creditCard.LimitLeft >= amount)
                    {
                        creditCard.Withdraw(amount);
                        amount = 0;
                    }

                    else
                    {
                        amount -= creditCard.LimitLeft;
                        creditCard.Withdraw(creditCard.LimitLeft);
                    }

                    if (amount == 0)
                    {
                        return;
                    }
                }
            }

            else
            {
                Console.WriteLine("Insufficient funds!");
            }
        }

        private static void GetInfo(User user)
        {
            Console.WriteLine($"User: {user.FirstName} {user.LastName}");
            Console.WriteLine("Bank Accounts:");

            var bankAccounts = user.PaymentMethods
                .Where(u => u.BankAccount != null)
                .Select(u => u.BankAccount);

            foreach (var bankAccount in bankAccounts)
            {
                Console.WriteLine($"-- ID: {bankAccount.BankAccountId}");
                Console.WriteLine($"--- Balance: {bankAccount.Balance:F2}");
                Console.WriteLine($"--- Bank: {bankAccount.BankName}");
                Console.WriteLine($"--- SWIFT: {bankAccount.SwiftCode}");
            }

            Console.WriteLine("Bank Accounts:");

            var creditCards = user.PaymentMethods
                .Where(u => u.CreditCard != null)
                .Select(u => u.CreditCard);

            foreach (var creditCard in creditCards)
            {
                Console.WriteLine($"-- ID: {creditCard.CreditCardId}");
                Console.WriteLine($"--- Limit: {creditCard.Limit:F2}");
                Console.WriteLine($"--- Money Owed: {creditCard.MoneyOwed:F2}");
                Console.WriteLine($"--- Limit Left: {creditCard.LimitLeft:F2}");
                Console.WriteLine($"--- Expiration Date: {creditCard.ExpirationDate.ToString("yyyy/MM")}");
            }
        }

        private static User GetUser(BillsPaymentSystemContext context)
        {
            int userId = int.Parse(Console.ReadLine());

            User user = null;

            while (true)
            {
                user = context.Users
                    .Where(u => u.UserId == userId)
                    .Include(u => u.PaymentMethods)
                    .ThenInclude(p => p.BankAccount)
                    .Include(u => u.PaymentMethods)
                    .ThenInclude(p => p.CreditCard)
                    .FirstOrDefault();

                if (user == null)
                {
                    userId = int.Parse(Console.ReadLine());
                    continue;
                }

                break;
            }

            return user;
        }
    }
}
