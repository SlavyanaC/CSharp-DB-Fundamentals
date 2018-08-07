namespace Instagraph.DataProcessor
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using AutoMapper.QueryableExtensions;
    using Instagraph.Data;
    using Instagraph.DataProcessor.Dtos.Export;
    using Newtonsoft.Json;

    public class Serializer
    {
        public static string ExportUncommentedPosts(InstagraphContext context)
        {
            var posts = context.Posts
                .Where(p => p.Comments.Count < 1)
                .OrderBy(p => p.Id)
                .Select(p => new
                {
                    p.Id,
                    Picture = p.Picture.Path,
                    User = p.User.Username,
                })
                .ToArray();

            var jsonString = JsonConvert.SerializeObject(posts, Newtonsoft.Json.Formatting.Indented);

            return jsonString;
        }

        public static string ExportPopularUsers(InstagraphContext context)
        {
            var users = context.Users
                .Where(user => user.Posts.Any(post => post.Comments
                       .Any(comment => user.Followers.Any(follower => follower.FollowerId == comment.UserId))))
                .OrderBy(u => u.Id)
                .ProjectTo<PopularUserDto>()
                .ToArray();

            var jsonString = JsonConvert.SerializeObject(users, Newtonsoft.Json.Formatting.Indented);

            return jsonString;
        }

        public static string ExportCommentsOnPosts(InstagraphContext context)
        {
            var sb = new StringBuilder();

            var users = context.Users
                .Select(u => new UserCommentsDto
                {
                    Username = u.Username,
                    MostComments = u.Posts.Count != 0 ? u.Posts.Max(p => p.Comments.Count) : 0 
                })
                .OrderByDescending(u => u.MostComments)
                .ThenBy(u => u.Username)
                .ToArray();

            var serializer = new XmlSerializer(typeof(UserCommentsDto[]), new XmlRootAttribute("users"));
            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            serializer.Serialize(new StringWriter(sb), users, namespaces);

            return sb.ToString();
        }
    }
}
