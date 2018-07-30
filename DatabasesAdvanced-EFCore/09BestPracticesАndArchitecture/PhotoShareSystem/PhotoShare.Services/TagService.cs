﻿namespace PhotoShare.Services
{
    using AutoMapper.QueryableExtensions;
    using PhotoShare.Data;
    using PhotoShare.Models;
    using PhotoShare.Services.Contracts;

    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class TagService : ITagService
    {
        private readonly PhotoShareContext context;

        public TagService(PhotoShareContext context)
        {
            this.context = context;
        }

        public Tag AddTag(string name)
        {
            Tag tag = new Tag
            {
                Name = name
            };

            this.context.Tags.Add(tag);
            this.context.SaveChanges();

            return tag;
        }

        public TModel ById<TModel>(int id) => this.By<TModel>(t => t.Id == id).SingleOrDefault();

        public TModel ByName<TModel>(string name) => this.By<TModel>(t => t.Name == name).SingleOrDefault();

        public bool Exists(int id) => this.ById<Tag>(id) != null;

        public bool Exists(string name) => this.ByName<Tag>(name) != null;

        private IEnumerable<TModel> By<TModel>(Func<Tag, bool> predicate) =>
            this.context.Tags
                .Where(predicate)
                .AsQueryable()
                .ProjectTo<TModel>();
    }
}
