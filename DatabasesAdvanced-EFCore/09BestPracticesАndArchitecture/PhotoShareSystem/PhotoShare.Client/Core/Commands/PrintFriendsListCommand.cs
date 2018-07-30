namespace PhotoShare.Client.Core.Commands
{
    using System;
    using System.Text;
    using PhotoShare.Client.Core.Contracts;
    using PhotoShare.Client.Core.Dtos;
    using PhotoShare.Services.Contracts;

    public class PrintFriendsListCommand : ICommand
    {
        private const string USER_NOT_FOUND = "User [user] not found!";
        private const string NO_FRIENDS = "No friends for this user. :(";

        private readonly IUserService userService;

        public PrintFriendsListCommand(IUserService userService)
        {
            this.userService = userService;
        }

        public string Execute(string[] data)
        {
            string username = data[0];

            bool exists = this.userService.Exists(username);
            if (!exists)
            {
                throw new ArgumentException(string.Format(USER_NOT_FOUND, username));
            }

            UserFriendsDto userFriendsDto = this.userService.ByUsername<UserFriendsDto>(username);

            if (userFriendsDto.Friends.Count == 0)
            {
                return NO_FRIENDS;
            }

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Friends:");
            foreach (var friend in userFriendsDto.Friends)
            {
                builder.AppendLine(friend.Username);
            }

            return builder.ToString().TrimEnd();
        }
    }
}
