namespace SoftJail.DataProcessor
{
    using Data;
    using System;
    using System.Linq;

    public class Bonus
    {
        public static string ReleasePrisoner(SoftJailDbContext context, int prisonerId)
        {
            var prisoner = context.Prisoners.SingleOrDefault(p => p.Id == prisonerId);
            var prisonerName = prisoner.FullName;

            if (prisoner.ReleaseDate == null)
            {
                return $"Prisoner {prisonerName} is sentenced to life";
            }

            var releaseDate = DateTime.Now;

            prisoner.ReleaseDate = releaseDate;
            prisoner.CellId = null;
            context.SaveChanges();

            var result = $"Prisoner {prisonerName} released";
            return result;
        }
    }
}
