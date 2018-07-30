namespace PhotoShare.Client.Core.Commands
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Contracts;
    using PhotoShare.Client.Core.Dtos;
    using PhotoShare.Services.Contracts;

    public class RegisterUserCommand : ICommand
    {
        private const string INVALID_DATA = "Invalid data!";
        private const string USERNAME_TAKEN_MESSAGE = "Username {0} is already taken!";
        private const string PASSWORD_DONT_MATCH_MESSAGE = "Passwords do not match!";
        private const string SUCCESSFULL_REGISTRATION = "User {0} was registered successfully!";

        private readonly IUserService userService;

        public RegisterUserCommand(IUserService userService)
        {
            this.userService = userService;
        }

        // RegisterUser <username> <password> <repeat-password> <email>
        public string Execute(string[] data)
        {
            string username = data[0];
            string password = data[1];
            string repeatPassword = data[2];
            string email = data[3];

            RegisterUserDto registerUserDto = new RegisterUserDto
            {
                Username = username,
                Password = password,
                Email = email,
            };

            if (!IsValid(registerUserDto))
            {
                throw new ArgumentException(INVALID_DATA);
            }

            if (this.userService.Exists(username))
            {
                throw new ArgumentException(string.Format(USERNAME_TAKEN_MESSAGE, username));
            }

            if (password != repeatPassword)
            {
                throw new InvalidOperationException(PASSWORD_DONT_MATCH_MESSAGE);
            }

            this.userService.Register(username, password, repeatPassword, email);

            string result = string.Format(SUCCESSFULL_REGISTRATION, username);
            return result;

        }

        private bool IsValid(object obj)
        {
            var validationContext = new ValidationContext(obj);
            var validationResults = new List<ValidationResult>();

            return Validator.TryValidateObject(obj, validationContext, validationResults, true);
        }
    }
}
