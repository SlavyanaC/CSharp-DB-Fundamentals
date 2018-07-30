namespace PhotoShare.Client.Core.Commands
{
    using System;

    using PhotoShare.Client.Core.Contracts;
    using PhotoShare.Services.Contracts;

    public class ActivateUserCommand : ICommand
    {
        private const string USER_NOT_FOUND = "User {0} not found!";
        private const string SUCCESSFULLY_ACTIVATED = "User {0} was successfully activated!";
        private const string LOGGED_ID = "You should logout first!";

        private readonly IUserSessionService userSessionService;
        private readonly IUserService userService;

        public ActivateUserCommand(IUserSessionService userSessionService, IUserService userService)
        {
            this.userSessionService = userSessionService;
            this.userService = userService;
        }

        // ActivateUser <username>
        public string Execute(string[] data)
        {
            string username = data[0];

            bool isLogged = this.userSessionService.IsLoggedIn();
            if (isLogged)
            {
                throw new ArgumentException(LOGGED_ID);
            }

            var userExists = this.userService.Exists(username);
            if (!userExists)
            {
                throw new ArgumentException(string.Format(USER_NOT_FOUND, username));
            }

            this.userService.Activate(username);
            return string.Format(SUCCESSFULLY_ACTIVATED, username);
        }
    }
}
