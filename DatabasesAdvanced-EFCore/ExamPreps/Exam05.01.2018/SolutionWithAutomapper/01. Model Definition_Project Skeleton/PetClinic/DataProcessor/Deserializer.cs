namespace PetClinic.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Text;

    using Newtonsoft.Json;

    using PetClinic.Models;
    using PetClinic.Data;
    using PetClinic.DataProcessor.Dto.Import;
    using System.Linq;
    using AutoMapper;
    using System.Xml.Serialization;
    using System.IO;
    using System.Globalization;

    public class Deserializer
    {
        private const string ERROR_MESSAGE = "Error: Invalid data.";

        public static string ImportAnimalAids(PetClinicContext context, string jsonString)
        {
            var animalAidDtos = JsonConvert.DeserializeObject<AnimalAidDto[]>(jsonString);

            var sb = new StringBuilder();
            var validObjects = new List<AnimalAid>();

            foreach (var animalAidDto in animalAidDtos)
            {
                var exists = validObjects.Any(aa => aa.Name == animalAidDto.Name);

                if (!IsValid(animalAidDto) || exists)
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                var animalAid = Mapper.Map<AnimalAid>(animalAidDto);

                validObjects.Add(animalAid);
                sb.AppendLine($"Record {animalAid.Name} successfully imported.");
            }

            context.AnimalAids.AddRange(validObjects);
            context.SaveChanges();

            var result = sb.ToString();
            return result;
        }

        public static string ImportAnimals(PetClinicContext context, string jsonString)
        {
            var animalDtos = JsonConvert.DeserializeObject<AnimalDto[]>(jsonString);

            var sb = new StringBuilder();
            var validObjects = new List<Animal>();

            foreach (var animalDto in animalDtos)
            {
                var passportExists = validObjects.Any(a => a.PassportSerialNumber == animalDto.Passport.SerialNumber);

                if (!IsValid(animalDto) || !IsValid(animalDto.Passport) || passportExists)
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                var animal = Mapper.Map<Animal>(animalDto);

                validObjects.Add(animal);
                sb.AppendLine($"Record {animal.Name} Passport №: {animal.Passport.SerialNumber} successfully imported.");
            }

            context.Animals.AddRange(validObjects);
            context.SaveChanges();

            var result = sb.ToString();
            return result;
        }

        public static string ImportVets(PetClinicContext context, string xmlString)
        {
            var serializer = new XmlSerializer(typeof(VetDto[]), new XmlRootAttribute("Vets"));
            var vetDtos = (VetDto[])serializer.Deserialize(new StringReader(xmlString));

            var sb = new StringBuilder();
            var validObjects = new List<Vet>();

            foreach (var vetDto in vetDtos)
            {
                var exists = validObjects.Any(v => v.PhoneNumber == vetDto.PhoneNumber);
                if (!IsValid(vetDto) || exists)
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                var vet = Mapper.Map<Vet>(vetDto);

                validObjects.Add(vet);
                sb.AppendLine($"Record {vet.Name} successfully imported.");
            }

            context.Vets.AddRange(validObjects);
            context.SaveChanges();

            var result = sb.ToString();
            return result;
        }

        public static string ImportProcedures(PetClinicContext context, string xmlString)
        {
            var serializer = new XmlSerializer(typeof(ProcedureDto[]), new XmlRootAttribute("Procedures"));
            var procedureDtos = (ProcedureDto[])serializer.Deserialize(new StringReader(xmlString));

            var sb = new StringBuilder();
            var validObjects = new List<Procedure>();

            foreach (var procedureDto in procedureDtos)
            {
                var animalAidsAreValid = procedureDto.AnimalAids.All(IsValid);

                if (!IsValid(procedureDto) || !animalAidsAreValid)
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                var vet = context.Vets.SingleOrDefault(v => v.Name == procedureDto.Vet);
                var animal = context.Animals.SingleOrDefault(a => a.PassportSerialNumber == procedureDto.Animal);

                if (vet == null || animal == null)
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                var validAnimalAids = context.AnimalAids
                    .Where(aa => procedureDto.AnimalAids.Any(daa => daa.Name == aa.Name))
                    .Select(aa => new ProcedureAnimalAidDto
                    {
                        Name = aa.Name,
                    })
                    .ToArray();

                bool oneOrMoreAidsDontExist = validAnimalAids.Count() < procedureDto.AnimalAids.Length;

                var distinctAids = procedureDto.AnimalAids.Distinct().ToArray();
                bool adIsGivenMoreThanOnce = distinctAids.Count() != procedureDto.AnimalAids.Length;

                if (oneOrMoreAidsDontExist || adIsGivenMoreThanOnce)
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                var dateTime = DateTime.ParseExact(procedureDto.DateTime, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                var procedure = new Procedure
                {
                    DateTime = dateTime,
                    Vet = vet,
                    Animal = animal,
                };

                var procedureAnimalAids = GenerateProcedureAnimalAids(context, procedure, validAnimalAids);
                procedure.ProcedureAnimalAids = procedureAnimalAids;

                validObjects.Add(procedure);
                sb.AppendLine("Record successfully imported.");
            }

            context.Procedures.AddRange(validObjects);
            context.SaveChanges();

            var result = sb.ToString();
            return result;
        }

        private static List<ProcedureAnimalAid> GenerateProcedureAnimalAids(PetClinicContext context, Procedure procedure, ProcedureAnimalAidDto[] validAnimalAids)
        {
            List<ProcedureAnimalAid> procedureAnimalAids = new List<ProcedureAnimalAid>();

            foreach (var animalAidDto in validAnimalAids)
            {
                var procedureAnimalAid = new ProcedureAnimalAid
                {
                    AnimalAid = context.AnimalAids.SingleOrDefault(aa => aa.Name == animalAidDto.Name),
                    Procedure = procedure
                };

                procedureAnimalAids.Add(procedureAnimalAid);
            }

            return procedureAnimalAids;
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
