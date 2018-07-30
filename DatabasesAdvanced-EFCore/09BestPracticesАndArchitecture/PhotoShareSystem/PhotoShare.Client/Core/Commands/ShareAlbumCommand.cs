namespace PhotoShare.Client.Core.Commands
{
    using System;

    using Contracts;
    using PhotoShare.Client.Core.Dtos;
    using PhotoShare.Client.Core.Exceptions;
    using PhotoShare.Models.Enums;
    using PhotoShare.Services.Contracts;

    public class ShareAlbumCommand : ICommand
    {
        private const string SUCCESS = "Username {0} added to album {1} ({2})";
        private const string ALBUM_NOT_FOUND = "Album {0} not found!";
        private const string USER_NOT_FOUND = "User {0} not found!";
        private const string INVALID_PERMISSION = "Permission must be either “Owner” or “Viewer”!";

        private readonly IUserSessionService userSessionService;
        private readonly IUserService userService;
        private readonly IAlbumService albumService;
        private readonly IAlbumRoleService albumRoleService;

        public ShareAlbumCommand(IUserSessionService userSessionService, IUserService userService, IAlbumService albumService, IAlbumRoleService albumRoleService)
        {
            this.userSessionService = userSessionService;
            this.userService = userService;
            this.albumService = albumService;
            this.albumRoleService = albumRoleService;
        }

        // ShareAlbum <albumId> <username> <permission>
        // For example:
        // ShareAlbum 4 dragon321 Owner
        // ShareAlbum 4 dragon11 Viewer
        public string Execute(string[] data)
        {
            int albumId = int.Parse(data[0]);
            string username = data[1];
            string permission = data[2];

            bool isLogged = this.userSessionService.IsLoggedIn();

            if (!isLogged)
            {
                throw new InvalidCredentialsException();
            }

            string albumName = this.albumService.ById<AlbumDto>(albumId).Name;
            bool albumExists = this.albumService.Exists(albumId);
            if (!albumExists)
            {
                throw new ArgumentException(string.Format(ALBUM_NOT_FOUND, albumName));
            }

            UserDto userDto = this.userService.ByUsername<UserDto>(username);
            bool userExists = this.userService.Exists(username);
            if (!userExists)
            {
                throw new ArgumentException(string.Format(USER_NOT_FOUND, userDto.Username));
            }

            var isValidPermission = Enum.TryParse(permission, out Role role);
            if (!isValidPermission)
            {
                throw new ArgumentException(INVALID_PERMISSION);
            }

            int userId = userDto.Id;
            this.albumRoleService.PublishAlbumRole(albumId, userId, permission);

            return string.Format(SUCCESS, username, albumName, permission);
        }
    }
}
