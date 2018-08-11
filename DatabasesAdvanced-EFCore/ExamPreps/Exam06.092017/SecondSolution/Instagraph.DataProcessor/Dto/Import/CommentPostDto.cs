using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Instagraph.DataProcessor.Dto.Import
{
    [XmlType("post")]
    public class CommentPostDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        [XmlAttribute("id")]
        public int Id { get; set; }
    }
}
