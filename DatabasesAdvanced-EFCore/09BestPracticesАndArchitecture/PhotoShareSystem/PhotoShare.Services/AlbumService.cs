namespace PhotoShare.Services
{
    using PhotoShare.Data;
    using PhotoShare.Models;
    using PhotoShare.Services.Contracts;
    using AutoMapper.QueryableExtensions;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using PhotoShare.Models.Enums;

    public class AlbumService : IAlbumService
    {
        private readonly PhotoShareContext context;

        public AlbumService(PhotoShareContext context)
        {
            this.context = context;
        }

        public Album Create(int userId, string albumTitle, string BgColor, string[] tags)
        {
            Color backgroundColor = Enum.Parse<Color>(BgColor, true);

            Album album = new Album
            {
                Name = albumTitle,
                BackgroundColor = backgroundColor,
            };

            this.context.Albums.Add(album);
            this.context.SaveChanges();

            AlbumRole albumRole = new AlbumRole
            {
                UserId = userId,
                Album = album,
            };

            this.context.AlbumRoles.Add(albumRole);
            this.context.SaveChanges();

            foreach (var tagName in tags)
            {
                int currentTagId = this.context.Tags.FirstOrDefault(t => t.Name == tagName).Id;
                AlbumTag albumTag = new AlbumTag
                {
                    Album = album,
                    TagId = currentTagId,
                };

                this.context.AlbumTags.Add(albumTag);
            }

            this.context.SaveChanges();
            return album;
        }

        public TModel ById<TModel>(int id) => this.By<TModel>(u => u.Id == id).SingleOrDefault();

        public TModel ByName<TModel>(string name) => this.By<TModel>(u => u.Name == name).SingleOrDefault();

        public bool Exists(int id) => this.ById<Album>(id) != null;

        public bool Exists(string name) => this.ByName<Album>(name) != null;

        private IEnumerable<TModel> By<TModel>(Func<Album, bool> predicate) =>
            this.context.Albums
                .Where(predicate)
                .AsQueryable()
                .ProjectTo<TModel>();
    }
}
