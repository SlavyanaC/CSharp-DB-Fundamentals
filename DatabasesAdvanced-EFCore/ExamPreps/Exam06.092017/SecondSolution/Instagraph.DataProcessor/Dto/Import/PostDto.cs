using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Instagraph.DataProcessor.Dto.Import
{
    [XmlType("post")]
    public class PostDto
    {
        [Required]
        [XmlElement("caption")]
        public string Caption { get; set; }

        [Required]
        [MaxLength(30)]
        [XmlElement("user")]
        public string User { get; set; }

        [Required]
        [MinLength(1)]
        [XmlElement("picture")]
        public string Picture { get; set; }
    }
}
