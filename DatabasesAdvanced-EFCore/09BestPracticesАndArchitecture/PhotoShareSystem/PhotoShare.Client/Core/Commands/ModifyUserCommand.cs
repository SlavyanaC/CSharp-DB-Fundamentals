namespace PhotoShare.Client.Core.Commands
{
    using System;
    using System.Linq;

    using Contracts;
    using PhotoShare.Client.Core.Dtos;
    using PhotoShare.Client.Core.Exceptions;
    using PhotoShare.Services.Contracts;

    public class ModifyUserCommand : ICommand
    {
        private const string PROPERTY_NOT_FOUND = "Property {0} not supported!";
        private const string USER_NOT_FOUND = "User {0} not found!";
        private const string TOWN_NOT_FOUND = "Town {0} not found!";

        private const string VALUE_NOT_VALID = "Value {0} not valid!";
        private const string INVALID_PASSWORD = "Invalid Password!";

        private const string SUCCESFULL_MODIFICATION = "User {0}'s {1} is {2}.";

        private readonly IUserSessionService userSessionService;
        private readonly IUserService userService;
        private readonly ITownService townService;

        public ModifyUserCommand(IUserSessionService userSessionService, IUserService userService, ITownService townService)
        {
            this.userSessionService = userSessionService;
            this.userService = userService;
            this.townService = townService;
        }

        // ModifyUser <username> <property> <new value>
        // For example:
        // ModifyUser <username> Password <NewPassword>
        // ModifyUser <username> BornTown <newBornTownName>
        // ModifyUser <username> CurrentTown <newCurrentTownName>
        // ModifyUser <username> FirstName <newFirstName>
        // ModifyUser <username> LastName <newLastNameName>
        // ModifyUser <username> Age <newAge>

        public string Execute(string[] data)
        {
            string username = data[0];
            string property = data[1];
            string newValue = data[2];

            bool isLogged = this.userSessionService.IsLoggedIn();
            if (!isLogged)
            {
                throw new InvalidCredentialsException();
            }

            UserDto userToModify = this.userService.ByUsername<UserDto>(username);
            int currentluLoggedUserId = this.userSessionService.User.Id;

            if (userToModify == null || currentluLoggedUserId != userToModify.Id)
            {
                throw new InvalidCredentialsException();
            }

            if (!this.userService.Exists(username) || userToModify.IsDeleted == true)
            {
                throw new ArgumentException(string.Format(USER_NOT_FOUND, username));
            }

            int userId = userToModify.Id;

            if (property == "Password")
            {
                SetPassword(userId, property, newValue);
            }
            else if (property == "BornTown")
            {
                SetBornTown(userId, property, newValue);
            }
            else if (property == "CurrentTown")
            {
                SetCurrentTown(userId, property, newValue);
            }
            else if (property == "FirstName")
            {
                this.userService.SetFirstName(userId, newValue);
            }
            else if (property == "LastName")
            {
                this.userService.SetLastName(userId, newValue);
            }
            else if (property == "Age")
            {
                this.userService.SetAge(userId, int.Parse(newValue));
            }
            else
            {
                throw new ArgumentException(string.Format(PROPERTY_NOT_FOUND, property));
            }

            string succesfullResult = string.Format(SUCCESFULL_MODIFICATION, username, property, newValue);
            return succesfullResult;
        }

        private void SetCurrentTown(int userId, string property, string currentTown)
        {
            bool townExists = this.townService.Exists(currentTown);
            if (!townExists)
            {
                throw new ArgumentException(string.Format(VALUE_NOT_VALID, property) + Environment.NewLine + string.Format(TOWN_NOT_FOUND, currentTown));
            }

            int townId = this.townService.ByName<TownDto>(currentTown).Id;

            this.userService.SetCurrentTown(userId, townId);
        }

        private void SetBornTown(int userId, string property, string bornTown)
        {
            bool townExists = this.townService.Exists(bornTown);
            if (!townExists)
            {
                throw new ArgumentException(string.Format(VALUE_NOT_VALID, property) + Environment.NewLine + string.Format(TOWN_NOT_FOUND, bornTown));
            }

            int townId = this.townService.ByName<TownDto>(bornTown).Id;

            this.userService.SetBornTown(userId, townId);
        }

        private void SetPassword(int userId, string property, string newPassword)
        {
            bool containsDigit = newPassword.Any(c => char.IsDigit(c));
            bool containsLower = newPassword.Any(c => char.IsLower(c));

            if (!containsDigit || !containsLower)
            {
                string errorMessage = string.Format(VALUE_NOT_VALID, newPassword) + Environment.NewLine + INVALID_PASSWORD;
                throw new ArgumentException(errorMessage);
            }

            this.userService.ChangePassword(userId, newPassword);
        }
    }
}
