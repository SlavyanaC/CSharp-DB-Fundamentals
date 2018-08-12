namespace SoftJail.DataProcessor
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Linq;
    using System.Text;
    using System.Globalization;
    using System.Xml.Serialization;

    using Newtonsoft.Json;

    using SoftJail.Data;
    using SoftJail.DataProcessor.ExportDto;

    public class Serializer
    {
        public static string ExportPrisonersByCells(SoftJailDbContext context, int[] ids)
        {
            var prisonerDtos = context.Prisoners
                .Where(p => ids.Any(i => i == p.Id))
                .Select(p => new PrisonerDto
                {
                    Id = p.Id,
                    Name = p.FullName,
                    CellNumber = p.Cell.CellNumber,
                    Officers = p.PrisonerOfficers.Select(po => new OfficerDto
                    {
                        Department = po.Officer.Department.Name,
                        OfficerName = po.Officer.FullName,
                    })
                    .OrderBy(o => o.OfficerName)
                    .ToArray(),
                    TotalOfficerSalary = p.PrisonerOfficers.Sum(po => po.Officer.Salary),
                })
                .OrderBy(p => p.Name)
                .ThenBy(p => p.Id)
                .ToArray();

            var jsonString = JsonConvert.SerializeObject(prisonerDtos, Newtonsoft.Json.Formatting.Indented);

            return jsonString;
        }

        public static string ExportPrisonersInbox(SoftJailDbContext context, string prisonersNames)
        {
            var names = prisonersNames.Split(',');

            Func<String, String> reverse = null;
            reverse = s => s.Length == 1 ? s : reverse(s.Substring(1)) + s[0];

            var prisonerMailDtos = context.Prisoners
                .Where(p => names.Any(n => n == p.FullName))
                .Select(p => new PrisonerMailDto
                {
                    Id = p.Id,
                    Name = p.FullName,
                    IncarcerationDate = p.IncarcerationDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    EncryptedMessages = p.Mails.Select(m => new MessageDto
                    {
                        Description = reverse(m.Description),
                    })
                    .ToArray(),
                })
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