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

    public class Deserializer
    {
        private const string ERROR_MESSAGE = "Invalid Data";

        public static string ImportDepartmentsCells(SoftJailDbContext context, string jsonString)
        {
            var DepartmentDtos = JsonConvert.DeserializeObject<DepartmentDto[]>(jsonString);

            var sb = new StringBuilder();
            var validDepartments = new List<Department>();

            foreach (var departmentDto in DepartmentDtos)
            {
                if (!IsValid(departmentDto))
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                var allCellsAreValid = departmentDto.Cells.All(IsValid);
                if (!allCellsAreValid)
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                var department = new Department
                {
                    Name = departmentDto.Name,
                };

                var validCells = new List<Cell>();
                foreach (var cellDto in departmentDto.Cells)
                {
                    var cell = new Cell
                    {
                        Department = department,
                        CellNumber = cellDto.CellNumber,
                        HasWindow = cellDto.HasWindow
                    };

                    validCells.Add(cell);
                }

                department.Cells = validCells;

                validDepartments.Add(department);

                sb.AppendLine($"Imported {department.Name} with {department.Cells.Count} cells");
            }

            context.Departments.AddRange(validDepartments);
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
                if (!IsValid(prisonerDto))
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                var mailsAreValid = prisonerDto.Mails.All(IsValid);

                if (!mailsAreValid)
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                var cell = context.Cells.SingleOrDefault(c => c.Id == prisonerDto.CellId);
                var incarcerationDate = DateTime.ParseExact(prisonerDto.IncarcerationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                var prisoner = new Prisoner
                {
                    FullName = prisonerDto.FullName,
                    Nickname = prisonerDto.Nickname,
                    Age = prisonerDto.Age,
                    Cell = cell,
                    IncarcerationDate = incarcerationDate,
                };

                if (prisonerDto.Bail != null)
                {
                    prisoner.Bail = prisonerDto.Bail;
                }

                if (prisonerDto.ReleaseDate != null)
                {
                    var releaseDate = DateTime.ParseExact(prisonerDto.IncarcerationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                    prisoner.ReleaseDate = releaseDate;
                }

                var validMails = new List<Mail>();
                foreach (var mailDto in prisonerDto.Mails)
                {
                    var mail = new Mail
                    {
                        Description = mailDto.Description,
                        Sender = mailDto.Sender,
                        Address = mailDto.Address,
                    };

                    validMails.Add(mail);
                }

                prisoner.Mails = validMails;

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
            var validOfficers = new List<Officer>();
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

                var department = context.Departments.SingleOrDefault(d => d.Id == officerDto.DepartmentId);

                var officer = new Officer
                {
                    FullName = officerDto.Name,
                    Position = position,
                    Weapon = weapon,
                    Department = department,
                    Salary = officerDto.Salary,
                };

                foreach (var prisonerDto in officerDto.Prisoners)
                {
                    var prisoner = context.Prisoners.SingleOrDefault(p => p.Id == prisonerDto.Id);

                    var officerPrisoner = new OfficerPrisoner
                    {
                        Officer = officer,
                        Prisoner = prisoner,
                    };

                    validOfficePrisoners.Add(officerPrisoner);

                }

                validOfficers.Add(officer);
                sb.AppendLine($"Imported {officer.FullName} ({officerDto.Prisoners.Length} prisoners)");
            }

            context.OfficersPrisoners.AddRange(validOfficePrisoners);
            context.Officers.AddRange(validOfficers);
            context.SaveChanges();

            var result = sb.ToString();
            return result;
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