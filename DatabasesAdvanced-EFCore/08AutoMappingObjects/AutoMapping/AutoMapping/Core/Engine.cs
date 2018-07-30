namespace AutoMapping.Core
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using AutoMapping.Core.Contracts;
    using AutoMapping.Services.Contracts;

    class Engine : IEngine
    {
        private readonly IServiceProvider serviceProvider;

        public Engine(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public void Run()
        {
            var initializeDb = this.serviceProvider.GetService<IEmployeeService>();
            initializeDb.InitializeDatabase();

            var commandParser = this.serviceProvider.GetService<ICommandParser>();

            while (true)
            {
                try
                {
                    string[] input = Console.ReadLine().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var result = commandParser.Parse(input);
                    Console.WriteLine(result);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }
            }
        }
    }
}
