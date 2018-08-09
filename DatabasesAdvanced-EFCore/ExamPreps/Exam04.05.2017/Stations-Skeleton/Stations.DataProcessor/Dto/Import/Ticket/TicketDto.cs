namespace Stations.DataProcessor.Dto.Import.Ticket
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("Ticket")]
    public class TicketDto
    {
        [Required]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        [XmlAttribute("price")]
        public decimal Price { get; set; }

        [Required]
        [MaxLength(8)]
        [RegularExpression(@"^[A-Z]{2}\d+$")]
        [XmlAttribute("seat")]
        public string SeatingPlace { get; set; }

        [Required]
        public TicketTripDto Trip { get; set; }

        public TicketCardDto Card { get; set; }
    }
}
