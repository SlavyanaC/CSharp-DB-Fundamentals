namespace PhotoShare.Client.Core.Commands
{
    using System;

    using PhotoShare.Client.Core.Contracts;
    using PhotoShare.Client.Core.Dtos;
    using PhotoShare.Services.Contracts;

    public class LoginCommand : ICommand
    {
        private const string SUCCESS = "User {0} successfully logged in!";
        private const string INVALID_USERNAME_OR_PASS = "Invalid username or password!";
        private const string ALREADY_LOGGED_ID = "You should logout first!";

        private readonly IUserService userService;
        private readonly IUserSessionService userSessionService;

        public LoginCommand(IUserService userService, IUserSessionService userSessionService)
        {
            this.userService = userService;
            this.userSessionService = userSessionService;
        }

        //Login <username> <password>
        public string Execute(string[] data)
        {
            string username = data[0];
            string password = data[1];

            UserDto user = this.userService.ByUsernameAndPassword<UserDto>(username, password);
            if (user == null || user.Password != password)
            {
                throw new ArgumentException(INVALID_USERNAME_OR_PASS);
            }

            bool isLogged = this.userSessionService.IsLoggedIn();
            if (isLogged)
            {
                throw new ArgumentException(ALREADY_LOGGED_ID);
            }

            this.userSessionService.LogIn(username);

            return string.Format(SUCCESS, username);
        }
    }
}
