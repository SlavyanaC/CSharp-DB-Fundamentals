namespace PhotoShare.Services
{
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using PhotoShare.Data;
    using PhotoShare.Models;
    using PhotoShare.Services.Contracts;

    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class UserService : IUserService
    {
        private readonly PhotoShareContext context;

        public UserService(PhotoShareContext context)
        {
            this.context = context;
        }

        public User Register(string username, string password, string repeatPassword, string email)
        {
            User user = new User
            {
                Username = username,
                Password = password,
                Email = email,
                IsDeleted = false,
                LastTimeLoggedIn = DateTime.Now,
                RegisteredOn = DateTime.Now
            };

            this.context.Users.Add(user);
            this.context.SaveChanges();

            return user;
        }

        public Friendship AddFriend(int userId, int friendId)
        {
            Friendship friendship = new Friendship
            {
                UserId = userId,
                FriendId = friendId
            };

            this.context.Friendships.Add(friendship);
            this.context.SaveChanges();

            return friendship;
        }

        public Friendship AcceptFriend(int userId, int friendId)
        {
            Friendship friendship = new Friendship
            {
                UserId = userId,
                FriendId = friendId
            };

            this.context.Friendships.Add(friendship);
            this.context.SaveChanges();

            return friendship;
        }

        public void ChangePassword(int userId, string password)
        {
            User user = this.context.Users.Find(userId);
            user.Password = password;
            this.context.SaveChanges();
        }

        public void SetBornTown(int userId, int townId)
        {
            User user = this.context.Users.Find(userId);
            user.BornTownId = townId;
            this.context.SaveChanges();
        }

        public void SetCurrentTown(int userId, int townId)
        {
            User user = this.context.Users.Find(userId);
            user.CurrentTownId = townId;
            this.context.SaveChanges();
        }

        public void SetFirstName(int userId, string newFirstName)
        {
            User user = this.context.Users.Find(userId);
            user.FirstName = newFirstName;
            this.context.SaveChanges();
        }

        public void SetLastName(int userId, string newLastName)
        {
            User user = this.context.Users.Find(userId);
            user.LastName = newLastName;
            this.context.SaveChanges();
        }

        public void SetAge(int userId, int newAge)
        {
            User user = this.context.Users.Find(userId);
            user.Age = newAge;
            this.context.SaveChanges();
        }

        public void Delete(string username)
        {
            User user = this.context.Users.First(u => u.Username == username);
            user.IsDeleted = true;
            this.context.SaveChanges();
        }

        public void Activate(string username)
        {
            User user = this.context.Users.First(u => u.Username == username);
            user.IsDeleted = false;
            this.context.SaveChanges();
        }

        public TModel ById<TModel>(int id) => this.By<TModel>(u => u.Id == id).SingleOrDefault();

        public TModel ByUsername<TModel>(string username) => this.By<TModel>(U => U.Username == username).SingleOrDefault();

        public TModel ByUsernameAndPassword<TModel>(string username, string password) => By<TModel>(a => a.Username == username && a.Password == password).SingleOrDefault();

        public bool Exists(int id) => this.ById<User>(id) != null;

        public bool Exists(string username) => this.ByUsername<User>(username) != null;

        private IEnumerable<TModel> By<TModel>(Func<User, bool> predicate) =>
            this.context.Users
                .Include(u => u.FriendsAdded)
                .Where(predicate)
                .AsQueryable()
                .ProjectTo<TModel>();
    }
}