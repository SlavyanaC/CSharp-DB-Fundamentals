namespace PhotoShare.Client.Core.Commands
{
    using System;
    using System.Linq;

    using Contracts;
    using PhotoShare.Client.Core.Dtos;
    using PhotoShare.Client.Core.Exceptions;
    using PhotoShare.Client.Utilities;
    using PhotoShare.Models.Enums;
    using Services.Contracts;

    public class CreateAlbumCommand : ICommand
    {
        private const string SUCCESSFUL_MESSAGE = "Album {0} successfully created!";
        private const string USER_NOT_FOUND = "User {0} not found!";
        private const string ALBUM_EXISTS = "Album {0} exists!";
        private const string COLOR_NOT_FOUND = "Color {0} not found!";
        private const string INVALID_TAGS = "Invalid tags!";

        private readonly IUserSessionService userSessionService;
        private readonly IAlbumService albumService;
        private readonly IUserService userService;
        private readonly ITagService tagService;

        public CreateAlbumCommand(IUserSessionService userSessionService, IAlbumService albumService, IUserService userService, ITagService tagService)
        {
            this.userSessionService = userSessionService;
            this.albumService = albumService;
            this.userService = userService;
            this.tagService = tagService;
        }

        // CreateAlbum <username> <albumTitle> <BgColor> <tag1> <tag2>...<tagN>
        public string Execute(string[] data)
        {
            string username = data[0];
            string albumTitle = data[1];
            string bgColor = data[2];
            string[] tags = data.Skip(3).ToArray();

            bool isLogged = this.userSessionService.IsLoggedIn();

            if (!isLogged)
            {
                throw new InvalidCredentialsException();
            }

            bool userExists = this.userService.Exists(username);
            if (!userExists)
            {
                throw new ArgumentException(string.Format(USER_NOT_FOUND, username));
            }

            bool albumExists = this.albumService.Exists(albumTitle);
            if (albumExists)
            {
                throw new ArgumentException(string.Format(ALBUM_EXISTS, albumTitle));
            }

            var isValidColor = Enum.TryParse(bgColor, out Color color);

            if (!isValidColor)
            {
                throw new ArgumentException(string.Format(COLOR_NOT_FOUND, bgColor));
            }

            for (int i = 0; i < tags.Length; i++)
            {
                tags[i] = tags[i].ValidateOrTransform();
                bool tagExists = this.tagService.Exists(tags[0]);
                if (!tagExists)
                {
                    throw new ArgumentException(INVALID_TAGS);
                }
            }

            int userId = this.userService.ByUsername<UserDto>(username).Id;
            this.albumService.Create(userId, albumTitle, bgColor, tags);

            return string.Format(SUCCESSFUL_MESSAGE, albumTitle);
        }
    }
}
