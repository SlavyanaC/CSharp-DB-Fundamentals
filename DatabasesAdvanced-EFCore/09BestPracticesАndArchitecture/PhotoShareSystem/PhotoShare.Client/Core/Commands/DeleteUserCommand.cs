namespace PhotoShare.Client.Core.Commands
{
    using System;

    using Dtos;
    using Contracts;
    using Services.Contracts;
    using PhotoShare.Client.Core.Exceptions;

    public class DeleteUserCommand : ICommand
    {
        private const string USER_NOT_FOUND = "User {0} not found!";
        private const string SUCCESSFULLY_DELETED = "User {0} was deleted from the database!";

        private readonly IUserSessionService userSessionService;
        private readonly IUserService userService;

        public DeleteUserCommand(IUserSessionService userSessionService, IUserService userService)
        {
            this.userSessionService = userSessionService;
            this.userService = userService;
        }

        // DeleteUser <username>
        public string Execute(string[] data)
        {
            string username = data[0];

            bool isLogged = this.userSessionService.IsLoggedIn();
            if (!isLogged)
            {
                throw new InvalidCredentialsException();
            }

            UserDto userDto = this.userService.ByUsername<UserDto>(username);
            int currentlyLoggedUserId = this.userSessionService.User.Id;

            if (userDto == null ||currentlyLoggedUserId != userDto.Id)
            {
                throw new InvalidCredentialsException();
            }

            var userExists = this.userService.Exists(username);
            if (!userExists)
            {
                throw new ArgumentException(string.Format(USER_NOT_FOUND, username));
            }

            if (userDto.IsDeleted == true)
            {
                throw new ArgumentException(string.Format(USER_NOT_FOUND, username));
            }

            this.userService.Delete(username);
            return string.Format(SUCCESSFULLY_DELETED, username);
        }
    }
}
