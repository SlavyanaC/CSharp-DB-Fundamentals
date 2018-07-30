namespace P01_BillsPaymentSystem.Initializer
{
    using P01_BillsPaymentSystem.Data.Models;

    public class UserInitializer
    {
        public static User[] GetUsers()
        {
            User[] users = new User[]
            {
                new User() { FirstName = "Pesho", LastName = "Goshev", Email = "pesho@mail.bg", Password = "1234" },
                new User() { FirstName = "Gosho", LastName = "Peshov", Email = "gosho@mail.bg", Password = "12345" },
                new User() { FirstName = "Penka", LastName = "Gosheva", Email = "penka@mail.bg", Password = "123456" },
                new User() { FirstName = "Spaska", LastName = "Ivanova", Email = "spaska@mail.bg", Password = "1234567" },
                new User() { FirstName = "Maria", LastName = "Gotseva", Email = "maria@mail.bg", Password = "12345678" },
                new User() { FirstName = "Ivan", LastName = "Yurukov", Email = "ivan@mail.bg", Password = "123456789" },
                new User() { FirstName = "Dragan", LastName = "Georgiev", Email = "dragan@mail.bg", Password = "1234567890" },
                new User() { FirstName = "Spas", LastName = "Martinov", Email = "spas@mail.bg", Password = "12345" },
                new User() { FirstName = "Silvia", LastName = "Evstatieva", Email = "silvia@mail.bg", Password = "123456" },
            };

            return users;
        }
    }
}
