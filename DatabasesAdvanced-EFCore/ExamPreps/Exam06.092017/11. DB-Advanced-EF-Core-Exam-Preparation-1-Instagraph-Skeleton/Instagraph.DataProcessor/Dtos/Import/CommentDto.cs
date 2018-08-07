namespace Instagraph.DataProcessor.Dtos.Import
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("comment")]
    public class CommentDto
    {
        [Required]
        [MaxLength(250)]
        [XmlElement("content")]
        public string Content { get; set; }

        [Required]
        [MaxLength(30)]
        [XmlElement("user")]
        public string UserName { get; set; }

        [Required]
        [XmlElement("post")]
        public CommentPostDto Post { get; set; }
    }
}
