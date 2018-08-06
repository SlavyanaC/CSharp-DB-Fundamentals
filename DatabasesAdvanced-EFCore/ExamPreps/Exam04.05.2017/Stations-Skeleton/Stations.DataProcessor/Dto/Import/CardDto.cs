namespace Stations.DataProcessor.Dto.Import
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("Card")]
    public class CardDto
    {
        public CardDto()
        {
            this.CardType = "Normal";
        }

        [Required]
        [MaxLength(128)]
        [XmlElement("Name")]
        public string Name { get; set; }

        [Range(0, 120)]
        [XmlElement("Age")]
        public int Age { get; set; }

        [XmlElement("CardType")]
        public string CardType { get; set; }
    }
}
