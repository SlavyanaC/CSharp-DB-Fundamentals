namespace PhotoShare.Client.Core.Commands
{
    using PhotoShare.Client.Core.Contracts;
    using PhotoShare.Services.Contracts;
    using System;

    public class LogoutCommand : ICommand
    {
        private const string SUCCESS = "User {0} successfully logged out!";
        private const string NOT_LOGGED = "You should log in first in order to logout.";

        private readonly IUserSessionService userSessionService;

        public LogoutCommand(IUserSessionService userSessionService)
        {
            this.userSessionService = userSessionService;
        }

        public string Execute(string[] data)
        {
            bool isLoggedIn = this.userSessionService.IsLoggedIn();
            if (!isLoggedIn)
            {
                throw new InvalidOperationException(NOT_LOGGED);
            }

            string username = this.userSessionService.GetUsername();
            this.userSessionService.LogOut();

            return string.Format(SUCCESS, username);
        }
    }
}
