namespace PhotoShare.Client.Core.Commands
{
    using System;
    using Dtos;
    using Contracts;
    using Services.Contracts;
    using PhotoShare.Client.Core.Exceptions;

    public class UploadPictureCommand : ICommand
    {
        private const string SUCCESSFUL_UPLOAD = "Picture {0} added to {1}!";
        private const string ALBUM_NOT_FOUND = "Album {0} not found!";

        private readonly IUserSessionService userSessionService;
        private readonly IPictureService pictureService;
        private readonly IAlbumService albumService;

        public UploadPictureCommand(IUserSessionService userSessionService, IPictureService pictureService, IAlbumService albumService)
        {
            this.userSessionService = userSessionService;
            this.pictureService = pictureService;
            this.albumService = albumService;
        }

        // UploadPicture <albumName> <pictureTitle> <pictureFilePath>
        public string Execute(string[] data)
        {
            string albumName = data[0];
            string pictureTitle = data[1];
            string path = data[2];

            bool isLogged = this.userSessionService.IsLoggedIn();
            if (!isLogged)
            {
                throw new InvalidCredentialsException();
            }

            var albumExists = this.albumService.Exists(albumName);

            if (!albumExists)
            {
                throw new ArgumentException(string.Format(ALBUM_NOT_FOUND, albumName));
            }

            var albumId = this.albumService.ByName<AlbumDto>(albumName).Id;

            var picture = this.pictureService.Create(albumId, pictureTitle, path);

            return string.Format(SUCCESSFUL_UPLOAD, pictureTitle, albumName);
        }
    }
}
