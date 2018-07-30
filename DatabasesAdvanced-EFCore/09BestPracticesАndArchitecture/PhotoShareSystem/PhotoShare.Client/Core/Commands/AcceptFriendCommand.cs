namespace PhotoShare.Client.Core.Commands
{
    using System;
    using System.Linq;
    using Contracts;
    using PhotoShare.Client.Core.Dtos;
    using PhotoShare.Client.Core.Exceptions;
    using PhotoShare.Services.Contracts;

    public class AcceptFriendCommand : ICommand
    {
        private const string SUCCESSFULLY_ACCEPTED = "{0} accepted {1} as a friend";
        private const string USER_NOT_FOUND = "{0} not found!";
        private const string ALREADY_FRIENDS = "{0} is already a friend to {1}";
        private const string REQUEST_NOT_SENT = "{0} has not added {1} as a friend";

        private readonly IUserSessionService userSessionService;
        private readonly IUserService userService;

        public AcceptFriendCommand(IUserSessionService userSessionService, IUserService userService)
        {
            this.userSessionService = userSessionService;
            this.userService = userService;
        }

        // AcceptFriend <username1> <username2>
        public string Execute(string[] data)
        {
            string username = data[0];
            string friendName = data[1];

            bool isLogged = this.userSessionService.IsLoggedIn();
            if (!isLogged)
            {
                throw new InvalidCredentialsException();
            }

            UserDto user = this.userService.ByUsername<UserDto>(username);
            int currentlyLoggedUserId = this.userSessionService.User.Id;

            if (user == null || currentlyLoggedUserId != user.Id)
            {
                throw new InvalidCredentialsException();
            }

            bool userExists = this.userService.Exists(username);
            if (!userExists || user.IsDeleted == true)
            {
                throw new ArgumentException(string.Format(USER_NOT_FOUND, username));
            }

            UserDto friend = this.userService.ByUsername<UserDto>(friendName);
            bool friendExists = this.userService.Exists(friendName);
            if (!friendExists || friend.IsDeleted == true)
            {
                throw new ArgumentException(string.Format(USER_NOT_FOUND, friendName));
            }

            UserFriendsDto userFriendsDto = this.userService.ByUsername<UserFriendsDto>(username);
            UserFriendsDto friedFriendsDto = this.userService.ByUsername<UserFriendsDto>(friendName);

            bool userHasSentrequest = userFriendsDto.Friends.Any(f => f.Username == friedFriendsDto.Username);
            bool friendHasSentRequest = friedFriendsDto.Friends.Any(f => f.Username == userFriendsDto.Username);

            if (userHasSentrequest && friendHasSentRequest)
            {
                throw new InvalidOperationException(string.Format(ALREADY_FRIENDS, friendName, username));
            }
            else if (!friendHasSentRequest)
            {
                throw new InvalidOperationException(string.Format(REQUEST_NOT_SENT, friendName, username));
            }

            int userId = user.Id;
            int friendId = friend.Id;
            this.userService.AcceptFriend(userId, friendId);

            return string.Format(SUCCESSFULLY_ACCEPTED, username, friendName);
        }
    }
}
