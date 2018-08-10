namespace Stations.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    using Newtonsoft.Json;
    using AutoMapper.QueryableExtensions;

    using Stations.Data;
    using Stations.DataProcessor.Dto.Export;
    using Stations.Models.Enums;

    public class Serializer
    {
        public static string ExportDelayedTrains(StationsDbContext context, string dateAsString)
        {
            var date = DateTime.ParseExact(dateAsString, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            var trains = context.Trains
                .Where(t => t.Trips.Any(tp => tp.Status == TripStatus.Delayed) && 
                       t.Trips.Any(tr => tr.DepartureTime <= date))
                .ProjectTo<DelayedTrainDto>()
                .OrderByDescending(t => t.DelayedTimes)
                .ThenByDescending(t => t.MaxDelayedTime)
                .ThenBy(t => t.TrainNumber)
                .ToArray();

            var jsonString = JsonConvert.SerializeObject(trains, Newtonsoft.Json.Formatting.Indented);

            return jsonString;
        }

        public static string ExportCardsTicket(StationsDbContext context, string cardType)
        {
            var type = Enum.Parse<CardType>(cardType);

            var cardDtos = context.Cards
                .Where(c => c.Type == type && c.BoughtTickets.Count > 0)
                .ProjectTo<CardDto>()
                .OrderBy(c => c.Name)
                .ToArray();

            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var serializer = new XmlSerializer(typeof(CardDto[]), new XmlRootAttribute("Cards"));

            var sb = new StringBuilder();
            serializer.Serialize(new StringWriter(sb), cardDtos, namespaces);

            return sb.ToString();
        }
    }
}