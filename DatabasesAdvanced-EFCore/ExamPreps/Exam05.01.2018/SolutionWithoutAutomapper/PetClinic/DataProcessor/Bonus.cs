namespace PetClinic.DataProcessor
{
    using System.Linq;

    using PetClinic.Data;
    using PetClinic.Models;

    public class Bonus
    {
        private const string ErrorMessage = "Vet with phone number {0} not found!";
        private const string SuccessMessage = "{0}'s profession updated from {1} to {2}.";

        public static string UpdateVetProfession(PetClinicContext context, string phoneNumber, string newProfession)
        {
            Vet vet = context.Vets.SingleOrDefault(v => v.PhoneNumber == phoneNumber);

            if (vet == null)
            {
                return string.Format(ErrorMessage, phoneNumber);
            }

            string oldProfession = vet.Profession;

            vet.Profession = newProfession;
            context.SaveChanges();

            string result = string.Format(SuccessMessage, vet.Name, oldProfession, newProfession);
            return result;
        }
    }
}
