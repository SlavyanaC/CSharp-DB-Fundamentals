namespace SoftJail.DataProcessor
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Globalization;
    using System.Xml.Serialization;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Newtonsoft.Json;

    using Data;
    using SoftJail.Data.Models;
    using SoftJail.Data.Models.Enums;
    using SoftJail.DataProcessor.ImportDto;
    using AutoMapper;

    public class Deserializer
    {
        private const string ERROR_MESSAGE = "Invalid Data";

        public static string ImportDepartmentsCells(SoftJailDbContext context, string jsonString)
        {
            var DepartmentDtos = JsonConvert.DeserializeObject<DepartmentDto[]>(jsonString);

            var sb = new StringBuilder();
            var validObjects = new List<Department>();

            foreach (var departmentDto in DepartmentDtos)
            {
                var allCellsAreValid = departmentDto.Cells.All(IsValid);

                if (!IsValid(departmentDto) || !allCellsAreValid)
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                var department = Mapper.Map<Department>(departmentDto);

                validObjects.Add(department);
                sb.AppendLine($"Imported {department.Name} with {department.Cells.Count} cells");
            }

            context.Departments.AddRange(validObjects);
            context.SaveChanges();

            var result = sb.ToString();
            return result;
        }

        public static string ImportPrisonersMails(SoftJailDbContext context, string jsonString)
        {
            var prisonerDtos = JsonConvert.DeserializeObject<PrisonerDto[]>(jsonString);

            var sb = new StringBuilder();
            var validPrisoners = new List<Prisoner>();

            foreach (var prisonerDto in prisonerDtos)
            {
                var mailsAreValid = prisonerDto.Mails.All(IsValid);
                if (!IsValid(prisonerDto) || !mailsAreValid)
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                var prisoner = Mapper.Map<Prisoner>(prisonerDto);

                if (prisonerDto.Bail != null)
                {
                    prisoner.Bail = prisonerDto.Bail;
                }

                if (prisonerDto.ReleaseDate != null)
                {
                    var releaseDate = DateTime.ParseExact(prisonerDto.IncarcerationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    prisoner.ReleaseDate = releaseDate;
                }

                validPrisoners.Add(prisoner);
                sb.AppendLine($"Imported {prisoner.FullName} {prisoner.Age} years old");
            }

            context.Prisoners.AddRange(validPrisoners);
            context.SaveChanges();

            var result = sb.ToString();
            return result;
        }

        public static string ImportOfficersPrisoners(SoftJailDbContext context, string xmlString)
        {
            var serializer = new XmlSerializer(typeof(OfficerDto[]), new XmlRootAttribute("Officers"));
            var officerDtos = (OfficerDto[])serializer.Deserialize(new StringReader(xmlString));

            var sb = new StringBuilder();
            var validObjects = new List<Officer>();
            var validOfficePrisoners = new List<OfficerPrisoner>();

            foreach (var officerDto in officerDtos)
            {
                if (!IsValid(officerDto))
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                var positionIsValid = Enum.TryParse(officerDto.Position, out Position position);
                var weaponIsValid = Enum.TryParse(officerDto.Weapon, out Weapon weapon);

                if (!positionIsValid || !weaponIsValid)
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                var officer = Mapper.Map<Officer>(officerDto);

                var officerPrisoners = GetOfficersPrisoners(context, officer, officerDto);
                validOfficePrisoners.AddRange(officerPrisoners);

                validObjects.Add(officer);
                sb.AppendLine($"Imported {officer.FullName} ({officerDto.Prisoners.Length} prisoners)");
            }

            context.OfficersPrisoners.AddRange(validOfficePrisoners);
            context.Officers.AddRange(validObjects);
            context.SaveChanges();

            var result = sb.ToString();
            return result;
        }

        private static List<OfficerPrisoner> GetOfficersPrisoners(SoftJailDbContext context, Officer officer, OfficerDto officerDto)
        {
            var officerPrisoners = new List<OfficerPrisoner>();

            foreach (var prisonerDto in officerDto.Prisoners)
            {
                var prisoner = context.Prisoners.SingleOrDefault(p => p.Id == prisonerDto.Id);

                var officerPrisoner = new OfficerPrisoner
                {
                    Officer = officer,
                    Prisoner = prisoner,
                };

                officerPrisoners.Add(officerPrisoner);
            }

            return officerPrisoners;
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);
            return isValid;
        }
    }
}