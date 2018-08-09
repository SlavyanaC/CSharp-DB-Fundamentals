namespace Stations.DataProcessor.Dto.Export
{
    using System.Xml.Serialization;

    [XmlType("Ticket")]
    public class TicketDto
    {
        public string OriginStation { get; set; }

        public string DestinationStation { get; set; }

        public string DepartureTime { get; set; }
    }
}