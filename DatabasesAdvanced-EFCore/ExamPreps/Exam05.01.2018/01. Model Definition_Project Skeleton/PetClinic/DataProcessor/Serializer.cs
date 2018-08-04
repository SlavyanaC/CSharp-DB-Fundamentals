namespace PetClinic.DataProcessor
{
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    using Json = Newtonsoft.Json;

    using PetClinic.Data;
    using PetClinic.DataProcessor.Dtos.Export;
    using System;
    using System.Globalization;

    public class Serializer
    {
        public static string ExportAnimalsByOwnerPhoneNumber(PetClinicContext context, string phoneNumber)
        {
            var animals = context.Animals
                .Where(a => a.Passport.OwnerPhoneNumber == phoneNumber)
                .OrderBy(a => a.Age)
                .ThenBy(a => a.PassportSerialNumber)
                .Select(a => new
                {
                    a.Passport.OwnerName,
                    AnimalName = a.Name,
                    a.Age,
                    SerialNumber = a.PassportSerialNumber,
                    RegisteredOn = a.Passport.RegistrationDate.ToString("dd-MM-yyy"),
                })
                .ToArray();

            var jsonString = Json.JsonConvert.SerializeObject(animals, Json.Formatting.Indented);

            return jsonString;
        }

        public static string ExportAllProcedures(PetClinicContext context)
        {
            var procedureExportDtos = context.Procedures
                .Select(p => new
                {
                    Passport = p.Animal.PassportSerialNumber,
                    OwnerNumber = p.Animal.Passport.OwnerPhoneNumber,
                    DateTime = p.DateTime,
                    AnimalAids = p.ProcedureAnimalAids.Select(paa => new ProcedureAnimaAidExportDto
                    {
                        Name = paa.AnimalAid.Name,
                        Price = paa.AnimalAid.Price,
                    })
                    .ToArray(),
                    TotalPrice = p.ProcedureAnimalAids.Sum(paa => paa.AnimalAid.Price),
                })
                .OrderBy(p => p.DateTime)
                .ThenBy(p => p.Passport)
                .Select(p => new ProcedureExportDto
                {
                    Passport = p.Passport,
                    OwnerNumber = p.OwnerNumber,
                    DateTime = p.DateTime.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture),
                    AnimalAids = p.AnimalAids,
                    TotalPrice = p.TotalPrice
                })
                .ToArray();

            StringBuilder sb = new StringBuilder();

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            XmlSerializer serializer = new XmlSerializer(typeof(ProcedureExportDto[]), new XmlRootAttribute("Procedures"));

            serializer.Serialize(new StringWriter(sb), procedureExportDtos, namespaces);

            return sb.ToString();
        }
    }
}
