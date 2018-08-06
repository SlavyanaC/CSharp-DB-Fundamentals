namespace Stations.DataProcessor.Dto.Import
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("Ticket")]
    public class TicketDto
    {
        [Required]
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        [XmlAttribute("price")]
        public decimal Price { get; set; }

        [Required]
        [RegularExpression(@"^\w{2}\d{1,6}$")]
        [XmlAttribute("seat")]
        public string Seat { get; set; }

        [Required]
        [XmlElement("Trip")]
        public TicketTripDto Trip { get; set; }

        [XmlElement("Card")]
        public TicketCardDto Card { get; set; }
    }
}
