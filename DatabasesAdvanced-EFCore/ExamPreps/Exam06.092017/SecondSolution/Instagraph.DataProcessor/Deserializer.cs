using System.IO;
using System.Text;
using System.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using AutoMapper;
using Newtonsoft.Json;

using Instagraph.Data;
using Instagraph.Models;
using Instagraph.DataProcessor.Dto.Import;

namespace Instagraph.DataProcessor
{
    public class Deserializer
    {
        private const string ERROR_MESSAGE = "Error: Invalid data.";

        public static string ImportPictures(InstagraphContext context, string jsonString)
        {
            var pictureDtos = JsonConvert.DeserializeObject<PictureDto[]>(jsonString);

            var sb = new StringBuilder();
            var validObjects = new List<Picture>();

            foreach (var pictureDto in pictureDtos)
            {
                var isAlreadyAdded = validObjects.Any(p => p.Path == pictureDto.Path);

                if (!IsValid(pictureDto) || isAlreadyAdded)
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                var picture = Mapper.Map<Picture>(pictureDto);

                validObjects.Add(picture);
                sb.AppendLine($"Successfully imported Picture {picture.Path}.");
            }

            context.Pictures.AddRange(validObjects);
            context.SaveChanges();

            var result = sb.ToString();
            return result;
        }

        public static string ImportUsers(InstagraphContext context, string jsonString)
        {
            var userDtos = JsonConvert.DeserializeObject<UserDto[]>(jsonString);

            var sb = new StringBuilder();
            var validObjects = new List<User>();

            foreach (var userDto in userDtos)
            {
                var alreadyAdded = validObjects.Any(u => u.Username == userDto.Username);

                if (!IsValid(userDto) || alreadyAdded)
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                var picture = context.Pictures.SingleOrDefault(p => p.Path == userDto.ProfilePicture);

                if (picture == null)
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                var user = Mapper.Map<User>(userDto);
                user.ProfilePicture = picture;

                validObjects.Add(user);
                sb.AppendLine($"Successfully imported User {user.Username}.");
            }

            context.Users.AddRange(validObjects);
            context.SaveChanges();

            var result = sb.ToString();
            return result;
        }

        public static string ImportFollowers(InstagraphContext context, string jsonString)
        {
            var userFollowerDtos = JsonConvert.DeserializeObject<UserFollowerDto[]>(jsonString);

            var sb = new StringBuilder();
            var validObjects = new List<UserFollower>();

            foreach (var userFollowerDto in userFollowerDtos)
            {
                var alreadyAdded = validObjects.Any(uf => uf.User.Username == userFollowerDto.User && uf.Follower.Username == userFollowerDto.Follower);

                if (!IsValid(userFollowerDto) || alreadyAdded)
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                var user = context.Users.SingleOrDefault(u => u.Username == userFollowerDto.User);
                var follower = context.Users.SingleOrDefault(u => u.Username == userFollowerDto.Follower);

                if (user == null || follower == null)
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                var userFollower = new UserFollower
                {
                    User = user,
                    Follower = follower,
                };

                validObjects.Add(userFollower);
                sb.AppendLine($"Successfully imported Follower {follower.Username} to User {user.Username}.");
            }

            context.UsersFollowers.AddRange(validObjects);
            context.SaveChanges();

            var result = sb.ToString();
            return result;
        }

        public static string ImportPosts(InstagraphContext context, string xmlString)
        {
            var serializer = new XmlSerializer(typeof(PostDto[]), new XmlRootAttribute("posts"));
            var postDtos = (PostDto[])serializer.Deserialize(new StringReader(xmlString));

            var sb = new StringBuilder();
            var validObjects = new List<Post>();

            foreach (var postDto in postDtos)
            {
                if (!IsValid(postDto))
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                var user = context.Users.SingleOrDefault(u => u.Username == postDto.User);
                var picture = context.Pictures.SingleOrDefault(p => p.Path == postDto.Picture);

                if (user == null || picture == null)
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                var post = new Post
                {
                    Caption = postDto.Caption,
                    User = user,
                    Picture = picture,
                };

                validObjects.Add(post);
                sb.AppendLine($"Successfully imported Post {post.Caption}.");
            }

            context.Posts.AddRange(validObjects);
            context.SaveChanges();

            var result = sb.ToString();
            return result;
        }

        public static string ImportComments(InstagraphContext context, string xmlString)
        {
            var serializer = new XmlSerializer(typeof(CommentDto[]), new XmlRootAttribute("comments"));
            var commentDtos = (CommentDto[])serializer.Deserialize(new StringReader(xmlString));

            var sb = new StringBuilder();
            var validObjects = new List<Comment>();

            foreach (var commentDto in commentDtos)
            {
                if (!IsValid(commentDto) || !IsValid(commentDto.Post))
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                var user = context.Users.SingleOrDefault(u => u.Username == commentDto.User);
                var post = context.Posts.SingleOrDefault(p => p.Id == commentDto.Post.Id);

                if (user == null || post == null)
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                var comment = new Comment
                {
                    Content = commentDto.Content,
                    User = user,
                    Post = post,
                };

                validObjects.Add(comment);
                sb.AppendLine($"Successfully imported Comment {comment.Content}.");
            }

            context.Comments.AddRange(validObjects);
            context.SaveChanges();

            var result = sb.ToString();
            return result;
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
