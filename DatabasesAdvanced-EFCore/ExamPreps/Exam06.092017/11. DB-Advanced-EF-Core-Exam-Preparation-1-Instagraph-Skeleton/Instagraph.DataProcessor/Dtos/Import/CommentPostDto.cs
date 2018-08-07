namespace Instagraph.DataProcessor.Dtos.Import
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("post")]
    public class CommentPostDto
    {
        [Required]
        [XmlAttribute("id")]
        public int Id { get; set; }
    }
}
