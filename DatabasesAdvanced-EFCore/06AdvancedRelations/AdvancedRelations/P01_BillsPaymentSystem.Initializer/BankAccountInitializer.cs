namespace P01_BillsPaymentSystem.Initializer
{
    using P01_BillsPaymentSystem.Data.Models;

    public class BankAccountInitializer
    {
        public static BankAccount[] GetBankAccounts()
        {
            BankAccount[] bankAccounts = new BankAccount[]
            {
                new BankAccount() { BankName = "First Bank", SwiftCode = "FBN", Balance = 1536.20m },
                new BankAccount() { BankName = "Second Bank", SwiftCode = "SBN", Balance = 152.15m },
                new BankAccount() { BankName = "A Bank", SwiftCode = "ABN", Balance = 35989.87m },
                new BankAccount() { BankName = "B Bank", SwiftCode = "BB", Balance =  1548.12m},
                new BankAccount() { BankName = "C Bank", SwiftCode = "CBN", Balance = 120.30m },
                new BankAccount() { BankName = "F Bank", SwiftCode = "FBN", Balance = 118m },
                new BankAccount() { BankName = "T Bank", SwiftCode = "TB", Balance = 845.23m },
                new BankAccount() { BankName = "Sofia Bank", SwiftCode = "SBN", Balance = 856.25m },
                new BankAccount() { BankName = "Plovdiv Bank", SwiftCode = "PBN", Balance = 852.36m },
            };

            return bankAccounts;
        }
    }
}
