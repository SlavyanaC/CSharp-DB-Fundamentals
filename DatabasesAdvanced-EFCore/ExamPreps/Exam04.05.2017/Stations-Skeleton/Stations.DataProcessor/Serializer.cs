namespace Stations.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    using Json = Newtonsoft.Json;

    using Stations.Data;
    using Stations.DataProcessor.Dto.Export;
    using Stations.Models.Enums;

    public class Serializer
    {
        public static string ExportDelayedTrains(StationsDbContext context, string dateAsString)
        {
            DateTime date = DateTime.ParseExact(dateAsString, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            var trains = context.Trains
                .Where(t => t.Trips.Any(tp => tp.Status == TripStatus.Delayed && tp.DepartureTime <= date))
                .Select(t => new
                {
                    t.TrainNumber,
                    DelayedTimes = t.Trips.Where(tp => tp.Status == TripStatus.Delayed).Count(),
                    MaxDelayedTime = t.Trips.Select(tp => tp.TimeDifference).Max()
                })
                .OrderByDescending(t => t.DelayedTimes)
                .ThenByDescending(t => t.MaxDelayedTime)
                .ToArray();

            string jsonString = Json.JsonConvert.SerializeObject(trains, Json.Formatting.Indented);

            return jsonString;
        }

        public static string ExportCardsTicket(StationsDbContext context, string cardType)
        {
            CardType type = Enum.Parse<CardType>(cardType);

            StringBuilder sb = new StringBuilder();

            CardDto[] cardDtos = context.Cards
                .Where(c => c.Type == type && c.BoughtTickets.Count > 0)
                .Select(c => new CardDto
                {
                    Name = c.Name,
                    Type = c.Type.ToString(),
                    Tickets = c.BoughtTickets.Select(t => new TicketDto
                    {
                        OriginStation = t.Trip.OriginStation.Name,
                        DestinationStation = t.Trip.DestinationStation.Name,
                        DepartureTime = t.Trip.DepartureTime.ToString("dd/MM/yyyy hh:mm", CultureInfo.InvariantCulture),
                    })
                    .ToArray(),
                })
                .OrderBy(c => c.Name)
                .ToArray();

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            XmlSerializer serializer = new XmlSerializer(typeof(CardDto[]), new XmlRootAttribute("Cards"));

            serializer.Serialize(new StringWriter(sb), cardDtos, namespaces);

            return sb.ToString();
        }
    }
}