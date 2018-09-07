namespace VaporStore.DataProcessor
{
    using System.Linq;
    using Data;
    using VaporStore.Data.Models;

    public static class Bonus
    {
        public static string UpdateEmail(VaporStoreDbContext context, string username, string newEmail)
        {
            User userWithUsername = context.Users
                .SingleOrDefault(u => u.Username == username);

            if (userWithUsername == null)
            {
                return $"User {username} not found";
            }

            User userWithEmail = context.Users
                .SingleOrDefault(u => u.Email == newEmail);

            if (userWithEmail != null)
            {
                return $"Email {newEmail} is already taken";
            }

            userWithUsername.Email = newEmail;
            context.SaveChanges();

            return $"Changed {username}'s email successfully";
        }
    }
}
