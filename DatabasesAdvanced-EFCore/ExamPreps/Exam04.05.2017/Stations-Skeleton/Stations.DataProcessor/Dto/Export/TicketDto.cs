namespace Stations.DataProcessor.Dto.Export
{
    using System.Xml.Serialization;

    [XmlType("Ticket")]
    public class TicketDto
    {
        [XmlElement("OriginStation")]
        public string OriginStation { get; set; }

        [XmlElement("DestinationStation")]
        public string DestinationStation { get; set; }

        [XmlElement("DepartureTime")]
        public string DepartureTime { get; set; }
    }
}
