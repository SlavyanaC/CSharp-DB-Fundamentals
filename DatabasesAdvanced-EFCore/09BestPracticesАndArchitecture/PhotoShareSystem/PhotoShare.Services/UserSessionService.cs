namespace PhotoShare.Services
{
    using PhotoShare.Models;
    using PhotoShare.Services.Contracts;

    public class UserSessionService : IUserSessionService
    {
        private readonly IUserService userService;

        public UserSessionService(IUserService userService)
        {
            this.userService = userService;
        }

        public User User { get; private set; }

        public void LogIn(string username)
        {
            this.User = userService.ByUsername<User>(username);
        }

        public void LogOut() => this.User = null;

        public bool IsLoggedIn() => this.User != null;

        public string GetUsername() => this.User.Username;
    }
}