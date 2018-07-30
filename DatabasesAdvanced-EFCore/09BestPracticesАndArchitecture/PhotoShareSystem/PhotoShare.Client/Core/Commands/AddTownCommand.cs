namespace PhotoShare.Client.Core.Commands
{
    using System;

    using Contracts;
    using Services.Contracts;
    using PhotoShare.Client.Core.Exceptions;

    public class AddTownCommand : ICommand
    {
        private const string TOWN_EXISTS = "Town {0} was already added!";
        private const string TOWN_ADDED_SUCCESSFULLY = "Town {0} was added successfully!";

        private readonly IUserSessionService userSessionService;
        private readonly ITownService townService;

        public AddTownCommand(IUserSessionService userSessionService, ITownService townService)
        {
            this.userSessionService = userSessionService;
            this.townService = townService;
        }

        // AddTown <townName> <countryName>
        public string Execute(string[] data)
        {
            string townName = data[0];
            string countryName = data[1];

            bool isLogged = this.userSessionService.IsLoggedIn();

            if (!isLogged)
            {
                throw new InvalidCredentialsException();
            }

            bool townExists = this.townService.Exists(townName);
            if (townExists)
            {
                throw new ArgumentException(string.Format(TOWN_EXISTS, townName));
            }

            this.townService.Add(townName, countryName);

            string successfullOutput = string.Format(TOWN_ADDED_SUCCESSFULLY, townName);
            return successfullOutput;
        }
    }
}
