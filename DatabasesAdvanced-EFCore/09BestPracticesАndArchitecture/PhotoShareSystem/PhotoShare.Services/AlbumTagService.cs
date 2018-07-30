﻿namespace PhotoShare.Services
{
    using PhotoShare.Data;
    using PhotoShare.Models;
    using PhotoShare.Services.Contracts;

    public class AlbumTagService : IAlbumTagService
    {
        private readonly PhotoShareContext context;

        public AlbumTagService(PhotoShareContext context)
        {
            this.context = context;
        }

        public AlbumTag AddTagTo(int albumId, int tagId)
        {
            AlbumTag albumTag = new AlbumTag
            {
                AlbumId = albumId,
                TagId = tagId
            };

            this.context.AlbumTags.Add(albumTag);
            this.context.SaveChanges();

            return albumTag;
        }
    }
}
