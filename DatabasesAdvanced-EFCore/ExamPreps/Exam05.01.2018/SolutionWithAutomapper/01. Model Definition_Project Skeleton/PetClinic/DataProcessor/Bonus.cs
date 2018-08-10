﻿namespace PetClinic.DataProcessor
{
    using System.Linq;
    using PetClinic.Data;

    public class Bonus
    {
        public static string UpdateVetProfession(PetClinicContext context, string phoneNumber, string newProfession)
        {
            var vet = context.Vets.SingleOrDefault(v => v.PhoneNumber == phoneNumber);

            if (vet == null)
            {
                return $"Vet with phone number {phoneNumber} not found!";
            }

            var oldProfession = vet.Profession;

            vet.Profession = newProfession;
            context.SaveChanges();

            var result = $"{vet.Name}'s profession updated from {oldProfession} to {newProfession}.";

            return result;
        }
    }
}
