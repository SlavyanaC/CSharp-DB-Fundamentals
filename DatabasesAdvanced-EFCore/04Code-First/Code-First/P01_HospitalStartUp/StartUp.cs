namespace P01_HospitalDatabaseStartUp
{
    using System;
    using P01_HospitalDatabase.Data;
    using P01_HospitalDatabase.Data.Models;
    using P01_HospitalDatabase.Initializer;

    public class StartUp
    {
        static void Main(string[] args)
        {
            //DatabaseInitializer.ResetDatabase();

            using (var context = new HospitalContext())
            {
                DatabaseInitializer.InitialSeed(context);
            }

            //using (var context = new HospitalContext())
            //{
            //    context.Database.EnsureCreated();
            //}
        }
    }
}
