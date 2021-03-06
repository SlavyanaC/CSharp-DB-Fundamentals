﻿namespace PhotoShare.Client.Core.Commands
{
    using System;
    using System.Linq;

    using Contracts;
    using PhotoShare.Client.Core.Dtos;
    using PhotoShare.Client.Core.Exceptions;
    using PhotoShare.Services.Contracts;

    public class AddFriendCommand : ICommand
    {
        private const string SUCCESSFULL_REQUEST = "Friend {0} added to {1}";
        private const string USER_NOT_FOUND = "{0} not found!";
        private const string ALREADY_FRIENDS = "{0} is already a friend to {1}";
        private const string REQUEST_SENT = "Request is already sent";

        private readonly IUserSessionService userSessionService;
        private readonly IUserService userService;

        public AddFriendCommand(IUserSessionService userSessionService, IUserService userService)
        {
            this.userSessionService = userSessionService;
            this.userService = userService;
        }

        // AddFriend <username1> <username2>
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

            bool userHasSentRequest = userFriendsDto.Friends.Any(f => f.Username == friedFriendsDto.Username);
            bool friendHasSentRequest = friedFriendsDto.Friends.Any(f => f.Username == userFriendsDto.Username);

            if (userHasSentRequest && friendHasSentRequest)
            {
                throw new InvalidOperationException(string.Format(ALREADY_FRIENDS, friendName, username));
            }
            else if (userHasSentRequest && !friendHasSentRequest)
            {
                throw new InvalidOperationException(REQUEST_SENT);
            }
            else if (!userHasSentRequest && friendHasSentRequest)
            {
                throw new InvalidOperationException(REQUEST_SENT);
            }

            int userId = user.Id;
            int friendId = friend.Id;
            this.userService.AddFriend(userId, friendId);

            return string.Format(SUCCESSFULL_REQUEST, friendName, username);
        }
    }
}
