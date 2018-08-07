namespace Instagraph.DataProcessor
{
    using System;
    using System.Text;
    using System.Linq;
    using System.Collections.Generic;
    using System.Xml.Linq;
    using System.ComponentModel.DataAnnotations;

    using Newtonsoft.Json;
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;

    using Instagraph.Data;
    using Instagraph.Models;
    using Instagraph.DataProcessor.Dtos.Import;
    using System.Xml.Serialization;
    using System.IO;

    public class Deserializer
    {
        private const string ERROR_MESSAGE = "Error: Invalid data.";

        public static string ImportPictures(InstagraphContext context, string jsonString)
        {
            var pictureDtos = JsonConvert.DeserializeObject<PictureDto[]>(jsonString);

            var sb = new StringBuilder();
            var validPictures = new List<Picture>();

            foreach (var pictureDto in pictureDtos)
            {
                if (!IsValid(pictureDto))
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                var picture = Mapper.Map<Picture>(pictureDto);

                validPictures.Add(picture);
                sb.AppendLine($"Successfully imported Picture {picture.Path}.");
            }

            context.Pictures.AddRange(validPictures);
            context.SaveChanges();

            return sb.ToString();
        }

        public static string ImportUsers(InstagraphContext context, string jsonString)
        {
            var userDtos = JsonConvert.DeserializeObject<UserDto[]>(jsonString);

            var sb = new StringBuilder();
            var validUsers = new List<User>();

            foreach (var userDto in userDtos)
            {
                var pictureExists = context.Pictures.SingleOrDefault(p => p.Path == userDto.ProfilePicturePath);
                if (!IsValid(userDto) || pictureExists == null)
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                var user = Mapper.Map<User>(userDto);
                user.ProfilePicture = pictureExists;

                validUsers.Add(user);
                sb.AppendLine($"Successfully imported User {user.Username}.");
            }

            context.Users.AddRange(validUsers);
            context.SaveChanges();

            return sb.ToString();
        }

        public static string ImportFollowers(InstagraphContext context, string jsonString)
        {
            var userFollowerDtos = JsonConvert.DeserializeObject<UserFollowerDto[]>(jsonString);

            var sb = new StringBuilder();
            var validUserFollowes = new List<UserFollower>();

            foreach (var userFollowerDto in userFollowerDtos)
            {
                var user = context.Users.SingleOrDefault(u => u.Username == userFollowerDto.User);
                var follower = context.Users.SingleOrDefault(f => f.Username == userFollowerDto.Follower);

                if (!IsValid(userFollowerDto) || user == null || follower == null)
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                bool alreadyAdded = validUserFollowes.Any(uf => uf.User == user && uf.Follower == follower);
                if (alreadyAdded)
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                var userFollower = Mapper.Map<UserFollower>(userFollowerDto);
                userFollower.User = user;
                userFollower.Follower = follower;

                validUserFollowes.Add(userFollower);
                sb.AppendLine($"Successfully imported Follower {follower.Username} to User {user.Username}.");
            }

            context.UsersFollowers.AddRange(validUserFollowes);
            context.SaveChanges();

            return sb.ToString();
        }

        public static string ImportPosts(InstagraphContext context, string xmlString)
        {
            var serializer = new XmlSerializer(typeof(PostDto[]), new XmlRootAttribute("posts"));
            var postDtos = (PostDto[])serializer.Deserialize(new StringReader(xmlString));

            var sb = new StringBuilder();
            var validPosts = new List<Post>();
            foreach (var postDto in postDtos)
            {
                var user = context.Users.SingleOrDefault(u => u.Username == postDto.UserName);
                var picture = context.Pictures.SingleOrDefault(p => p.Path == postDto.PicturePath);

                if (!IsValid(postDto) || user == null || picture == null)
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                var post = Mapper.Map<Post>(postDto);
                post.User = user;
                post.Picture = picture;

                validPosts.Add(post);
                sb.AppendLine($"Successfully imported Post {post.Caption}.");
            }

            context.Posts.AddRange(validPosts);
            context.SaveChanges();

            return sb.ToString();
        }

        public static string ImportComments(InstagraphContext context, string xmlString)
        {
            var serialized = new XmlSerializer(typeof(CommentDto[]), new XmlRootAttribute("comments"));

            var commentDtos = (CommentDto[])serialized.Deserialize(new StringReader(xmlString));

            var sb = new StringBuilder();
            var validComments = new List<Comment>();
            foreach (var commentDto in commentDtos)
            {
                if (!IsValid(commentDto))
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                var user = context.Users.SingleOrDefault(u => u.Username == commentDto.UserName);
                var post = context.Posts.SingleOrDefault(p => p.Id == commentDto.Post.Id);

                if (user == null || post == null)
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                var comment = Mapper.Map<Comment>(commentDto);
                comment.Post = post;
                comment.User = user;

                validComments.Add(comment);
                sb.AppendLine($"Successfully imported Comment {comment.Content}.");
            }

            context.Comments.AddRange(validComments);
            context.SaveChanges();

            return sb.ToString();
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);
            return isValid;
        }
    }
}
