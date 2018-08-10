namespace PetClinic.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using AutoMapper.QueryableExtensions;
    using Newtonsoft.Json;

    using PetClinic.Data;
    using PetClinic.DataProcessor.Dto.Export;

    public class Serializer
    {
        public static string ExportAnimalsByOwnerPhoneNumber(PetClinicContext context, string phoneNumber)
        {
            var Dtos = context.Animals
                .Where(a => a.Passport.OwnerPhoneNumber == phoneNumber)
                .ProjectTo<AnimalDto>()
                .OrderBy(a => a.Age)
                .ThenBy(a => a.SerialNumber);

            var jsonString = JsonConvert.SerializeObject(Dtos, Newtonsoft.Json.Formatting.Indented);

            return jsonString;
        }

        public static string ExportAllProcedures(PetClinicContext context)
        {
            var procedureDtos = context.Procedures
                .ProjectTo<ProcedureDto>()
                .OrderBy(p => DateTime.ParseExact(p.DateTime, "dd-MM-yyyy", CultureInfo.InvariantCulture))
                .ThenBy(p => p.Passport)
                .ToArray();

            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var serializer = new XmlSerializer(typeof(ProcedureDto[]), new XmlRootAttribute("Procedures"));

            var sb = new StringBuilder();
            serializer.Serialize(new StringWriter(sb), procedureDtos, namespaces);

            return sb.ToString();
        }
    }
}
