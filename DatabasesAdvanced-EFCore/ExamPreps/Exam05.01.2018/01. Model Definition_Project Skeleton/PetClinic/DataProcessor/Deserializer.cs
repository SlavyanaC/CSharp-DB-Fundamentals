namespace PetClinic.DataProcessor
{
    using System;
    using System.IO;
    using System.Text;
    using System.Linq;
    using System.Globalization;
    using System.Xml.Serialization;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    using PetClinic.Data;
    using PetClinic.Models;
    using PetClinic.DataProcessor.Dtos.Import;

    public class Deserializer
    {
        private const string ErrorMessage = "Error: Invalid data.";
        private const string SuccessMessage = "Record successfully imported.";
        private const string ItemSuccessMessage = "Record {0} successfully imported.";
        private const string AnimalSuccessMessgae = "Record {0} Passport №: {1} successfully imported.";

        public static string ImportAnimalAids(PetClinicContext context, string jsonString)
        {
            AnimalAid[] deserializedAnimalAids = JsonConvert.DeserializeObject<AnimalAid[]>(jsonString);

            List<AnimalAid> validAnimalAids = new List<AnimalAid>();
            StringBuilder sb = new StringBuilder();
            foreach (var deserializedObj in deserializedAnimalAids)
            {
                bool alreadyExists = validAnimalAids.Any(aa => aa.Name == deserializedObj.Name);

                if (!IsValid(deserializedObj) || alreadyExists)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                AnimalAid animalAid = new AnimalAid
                {
                    Name = deserializedObj.Name,
                    Price = deserializedObj.Price,
                };

                validAnimalAids.Add(animalAid);
                sb.AppendLine(string.Format(ItemSuccessMessage, animalAid.Name));
            }

            context.AnimalAids.AddRange(validAnimalAids);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportAnimals(PetClinicContext context, string jsonString)
        {
            var format = "dd-MM-yyyy";
            var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = format };

            Animal[] deserializedAnimals = JsonConvert.DeserializeObject<Animal[]>(jsonString, dateTimeConverter);

            List<Animal> validAnimals = new List<Animal>();
            StringBuilder sb = new StringBuilder();

            foreach (var deserializedAnimal in deserializedAnimals)
            {
                bool alreadyExists = validAnimals.Any(a => a.Passport.SerialNumber == deserializedAnimal.Passport.SerialNumber);
                if (!IsValid(deserializedAnimal) || !IsValid(deserializedAnimal.Passport) || alreadyExists)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Passport passport = new Passport
                {
                    SerialNumber = deserializedAnimal.Passport.SerialNumber,
                    OwnerName = deserializedAnimal.Passport.OwnerName,
                    OwnerPhoneNumber = deserializedAnimal.Passport.OwnerPhoneNumber,
                    RegistrationDate = deserializedAnimal.Passport.RegistrationDate,
                };

                Animal animal = new Animal
                {
                    Name = deserializedAnimal.Name,
                    Type = deserializedAnimal.Type,
                    Age = deserializedAnimal.Age,
                    Passport = passport,
                };

                validAnimals.Add(animal);
                sb.AppendLine(string.Format(AnimalSuccessMessgae, animal.Name, animal.Passport.SerialNumber));
            }

            context.Animals.AddRange(validAnimals);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportVets(PetClinicContext context, string xmlString)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(VetDto[]), new XmlRootAttribute("Vets"));

            VetDto[] vetDtos = (VetDto[])serializer.Deserialize(new StringReader(xmlString));

            StringBuilder sb = new StringBuilder();
            List<Vet> validVets = new List<Vet>();
            foreach (var vetDto in vetDtos)
            {
                bool alreadyExists = validVets.Any(v => v.PhoneNumber == vetDto.PhoneNumber);
                if (!IsValid(vetDto) || alreadyExists)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Vet vet = new Vet
                {
                    Name = vetDto.Name,
                    Age = vetDto.Age,
                    Profession = vetDto.Profession,
                    PhoneNumber = vetDto.PhoneNumber,
                };

                validVets.Add(vet);
                sb.AppendLine(string.Format(ItemSuccessMessage, vet.Name));
            }

            context.Vets.AddRange(validVets);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportProcedures(PetClinicContext context, string xmlString)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ProcedureImportDto[]), new XmlRootAttribute("Procedures"));

            ProcedureImportDto[] procedureDtos = (ProcedureImportDto[])serializer.Deserialize(new StringReader(xmlString));

            List<Procedure> validProcedures = new List<Procedure>();
            StringBuilder sb = new StringBuilder();
            foreach (var procedureDto in procedureDtos)
            {
                Vet vet = context.Vets.SingleOrDefault(v => v.Name == procedureDto.VetName);
                Animal animal = context.Animals.SingleOrDefault(a => a.PassportSerialNumber == procedureDto.AnimalName);

                if (!IsValid(procedureDto) || vet == null || animal == null)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                bool isError = false;
                List<AnimalAid> animalAids = new List<AnimalAid>();
                foreach (var animalAidDto in procedureDto.AnimalAids)
                {
                    AnimalAid animalAid = context.AnimalAids.SingleOrDefault(ai => ai.Name == animalAidDto.Name);

                    bool aidAlreadyGiven = animalAids.Any(ai => ai.Name == animalAidDto.Name);
                    if (!IsValid(animalAidDto) || animalAid == null || aidAlreadyGiven)
                    {
                        sb.AppendLine(ErrorMessage);
                        isError = true;
                        break;
                    }

                    animalAids.Add(animalAid);
                }

                if (isError)
                {
                    continue;
                }

                Procedure procedure = new Procedure
                {
                    Animal = animal,
                    Vet = vet,
                    DateTime = DateTime.ParseExact(procedureDto.DateTime, "dd-MM-yyyy", CultureInfo.InvariantCulture),
                };

                foreach (var animalAid in animalAids)
                {
                    ProcedureAnimalAid procedureAnimalAid = new ProcedureAnimalAid
                    {
                        AnimalAid = animalAid,
                        Procedure = procedure
                    };

                    procedure.ProcedureAnimalAids.Add(procedureAnimalAid);
                }

                validProcedures.Add(procedure);
                sb.AppendLine(SuccessMessage);
            }

            context.Procedures.AddRange(validProcedures);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        private static bool IsValid(object obj)
        {
            ValidationContext validationContext = new ValidationContext(obj);
            List<ValidationResult> validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);
            return isValid;
        }
    }
}
