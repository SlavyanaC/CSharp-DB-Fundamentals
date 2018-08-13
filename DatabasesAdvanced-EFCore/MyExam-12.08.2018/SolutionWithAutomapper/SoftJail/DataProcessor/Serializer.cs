namespace SoftJail.DataProcessor
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    using Newtonsoft.Json;

    using SoftJail.Data;
    using SoftJail.DataProcessor.ExportDto;
    using AutoMapper.QueryableExtensions;

    public class Serializer
    {
        public static string ExportPrisonersByCells(SoftJailDbContext context, int[] ids)
        {
            var prisonerDtos = context.Prisoners
                .Where(p => ids.Any(i => i == p.Id))
                .ProjectTo<PrisonerDto>()
                .OrderBy(p => p.Name)
                .ThenBy(p => p.Id)
                .ToArray();

            var jsonString = JsonConvert.SerializeObject(prisonerDtos, Newtonsoft.Json.Formatting.Indented);

            return jsonString;
        }

        public static string ExportPrisonersInbox(SoftJailDbContext context, string prisonersNames)
        {
            var names = prisonersNames.Split(',');

            var prisonerMailDtos = context.Prisoners
                .Where(p => names.Any(n => n == p.FullName))
                .ProjectTo<PrisonerMailDto>()
                .OrderBy(p => p.Name)
                .ThenBy(p => p.Id)
                .ToArray();

            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var serializer = new XmlSerializer(typeof(PrisonerMailDto[]), new XmlRootAttribute("Prisoners"));

            var sb = new StringBuilder();
            serializer.Serialize(new StringWriter(sb), prisonerMailDtos, namespaces);

            return sb.ToString();
        }
    }
}