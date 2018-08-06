namespace Stations.DataProcessor.Dto.Import
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("Trip")]
    public class TicketTripDto
    {
        [Required]
        [MaxLength(50)]
        [XmlElement("OriginStation")]
        public string OriginStationName { get; set; }

        [Required]
        [MaxLength(50)]
        [XmlElement("DestinationStation")]
        public string DestinationStationName { get; set; }

        [Required]
        [XmlElement("DepartureTime")]
        public string DepartureTime { get; set; }
    }
}
