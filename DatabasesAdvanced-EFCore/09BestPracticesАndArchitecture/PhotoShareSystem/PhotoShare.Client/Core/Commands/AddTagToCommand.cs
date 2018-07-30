namespace PhotoShare.Client.Core.Commands
{
    using System;

    using PhotoShare.Client.Core.Contracts;
    using PhotoShare.Client.Core.Dtos;
    using PhotoShare.Services.Contracts;
    using PhotoShare.Client.Utilities;
    using PhotoShare.Client.Core.Exceptions;

    public class AddTagToCommand : ICommand
    {
        private const string SUCCESSFULY_ADDED = "Tag {0} added to {1}!";
        private const string ALBUM_OR_TAG_NOT_FOUND = "Either tag or album do not exist!";

        private readonly IUserSessionService userSessionService;
        private readonly IAlbumService albumService;
        private readonly ITagService tagService;
        private readonly IAlbumTagService albumTagService;

        public AddTagToCommand(IUserSessionService userSessionService, IAlbumService albumService, ITagService tagService, IAlbumTagService albumTagService)
        {
            this.userSessionService = userSessionService;
            this.albumService = albumService;
            this.tagService = tagService;
            this.albumTagService = albumTagService;
        }

        // AddTagTo <albumName> <tag>
        public string Execute(string[] args)
        {
            string albumTitle = args[0];
            string tagName = args[1];

            bool isLogged = this.userSessionService.IsLoggedIn();
            if (!isLogged)
            {
                throw new InvalidCredentialsException();
            }

            string validTagName = tagName.ValidateOrTransform();

            bool albumExists = this.albumService.Exists(albumTitle);
            bool tagExists = this.tagService.Exists(validTagName);
            if (!albumExists || !tagExists)
            {
                throw new ArgumentException(ALBUM_OR_TAG_NOT_FOUND);
            }

            int albumId = this.albumService.ByName<AlbumDto>(albumTitle).Id;
            int tagId = this.tagService.ByName<TagDto>(validTagName).Id;

            this.albumTagService.AddTagTo(albumId, tagId);

            return string.Format(SUCCESSFULY_ADDED, validTagName, albumTitle);
        }
    }
}
