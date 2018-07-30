namespace P01_HospitalDatabase.Generators
{
	using System;
	
    class EmailGenerator
    {
        private static Random rnd = new Random();

        private static string[] domains = { "mail.bg", "abv.bg", "gmail.com", "hotmail.com", "softuni.bg", "students.softuni.bg" };

        internal static string NewEmail(string name)
        {
            string domain = domains[rnd.Next(domains.Length)];
            int number = rnd.Next(1, 2000);

            return $"{name.ToLower()}{number}@{domain}";
        }
    }
}
