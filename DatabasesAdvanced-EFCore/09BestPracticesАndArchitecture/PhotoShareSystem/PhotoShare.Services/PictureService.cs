namespace PhotoShare.Services
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using AutoMapper.QueryableExtensions;

    using Data;
    using Models;
    using Contracts;

    public class PictureService : IPictureService
    {
        private readonly PhotoShareContext context;

        public PictureService(PhotoShareContext context)
        {
            this.context = context;
        }

        public Picture Create(int albumId, string pictureTitle, string pictureFilePath)
        {
            var picture = new Picture()
            {
                AlbumId = albumId,
                Title = pictureTitle,
                Path = pictureFilePath,
            };

            this.context.Pictures.Add(picture);
            this.context.SaveChanges();

            return picture;
        }

        public TModel ById<TModel>(int id) => By<TModel>(a => a.Id == id).SingleOrDefault();

        public TModel ByTitle<TModel>(string name) => By<TModel>(a => a.Title == name).SingleOrDefault();

        public bool Exists(int id) => ById<Picture>(id) != null;

        public bool Exists(string name) => ByTitle<Picture>(name) != null;

        private IEnumerable<TModel> By<TModel>(Func<Picture, bool> predicate) =>
            this.context.Pictures
                .Where(predicate)
                .AsQueryable()
                .ProjectTo<TModel>();
    }
}
