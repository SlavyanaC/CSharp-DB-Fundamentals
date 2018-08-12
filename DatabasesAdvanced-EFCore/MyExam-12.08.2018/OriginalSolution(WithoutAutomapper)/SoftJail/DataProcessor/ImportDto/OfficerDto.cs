using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace SoftJail.DataProcessor.ImportDto
{
    [XmlType("Officer")]
    public class OfficerDto
    {
        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string Name { get; set; }

        [XmlElement("Money")]
        [Required]
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal Salary { get; set; }

        [Required]
        public string Position { get; set; }

        [Required]
        public string Weapon { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        [XmlArray("Prisoners")]
        public OfficerPrisonerDto[] Prisoners { get; set; }
    }
}
