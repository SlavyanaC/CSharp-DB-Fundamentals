using System.ComponentModel.DataAnnotations;
using System.Xml;
using System.Xml.Serialization;

namespace SoftJail.DataProcessor.ImportDto
{
    [XmlType("Prisoner")]
    public class OfficerPrisonerDto
    {
        [XmlAttribute("id")]
        [Required]
        public int Id { get; set; }
    }
}
