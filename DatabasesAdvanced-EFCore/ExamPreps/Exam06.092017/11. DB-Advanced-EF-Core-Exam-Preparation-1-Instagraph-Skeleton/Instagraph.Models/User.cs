﻿namespace Instagraph.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class User
    {
        public User()
        {
            this.Followers = new HashSet<UserFollower>();
            this.UsersFollowing = new HashSet<UserFollower>();
            this.Posts = new HashSet<Post>();
            this.Comments = new HashSet<Comment>();
        }

        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Username { get; set; }

        [Required]
        [MaxLength(20)]
        public string Password { get; set; }

        public int ProfilePictureId { get; set; }
        [Required]
        public Picture ProfilePicture { get; set; }

        public ICollection<UserFollower> Followers { get; set; }

        public ICollection<UserFollower> UsersFollowing { get; set; }

        public ICollection<Post> Posts { get; set; }

        public ICollection<Comment> Comments { get; set; }
    }
}
