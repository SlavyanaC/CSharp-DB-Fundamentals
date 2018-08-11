using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

using AutoMapper.QueryableExtensions;

using Instagraph.Data;
using Instagraph.DataProcessor.Dto.Export;
using Newtonsoft.Json;

namespace Instagraph.DataProcessor
{
    public class Serializer
    {
        public static string ExportUncommentedPosts(InstagraphContext context)
        {
            var Dtos = context.Posts
                .Where(p => p.Comments.Count == 0)
                .ProjectTo<PostDto>()
                .OrderBy(p => p.Id)
                .ToArray();

            var jsonString = JsonConvert.SerializeObject(Dtos, Newtonsoft.Json.Formatting.Indented);

            return jsonString;
        }

        public static string ExportPopularUsers(InstagraphContext context)
        {
            var Dtos = context.Users
                .Where(u => u.Posts.Any(p => p.Comments.Any(c => u.Followers.Any(f => f.FollowerId == c.UserId))))
                .OrderBy(u => u.Id)
                .ProjectTo<PopularUserDto>()
                .ToArray();

            var jsonString = JsonConvert.SerializeObject(Dtos, Newtonsoft.Json.Formatting.Indented);

            return jsonString;
        }

        public static string ExportCommentsOnPosts(InstagraphContext context)
        {
            var userCommentDtos = context.Users
                .ProjectTo<UserCommentDto>()
                .OrderByDescending(u => u.MostComments)
                .ThenBy(u => u.Username)
                .ToArray();

            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var serializer = new XmlSerializer(typeof(UserCommentDto[]), new XmlRootAttribute("users"));

            var sb = new StringBuilder();
            serializer.Serialize(new StringWriter(sb), userCommentDtos, namespaces);

            return sb.ToString();
        }
    }
}
