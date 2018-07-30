namespace AutoMapping
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using AutoMapper;
    using AutoMapping.Core;
    using AutoMapping.Core.Contracts;
    using AutoMapping.Data;
    using AutoMapping.Services;
    using AutoMapping.Services.Contracts;
    using AutoMapping.Core.Controllers.Contracts;
    using AutoMapping.Core.Controllers;

    class StartUp
    {
        static void Main()
        {
            IServiceProvider serviceProvider = ConfigureServices();
            IEngine Engine = new Engine(serviceProvider);
            Engine.Run();
        }

        static IServiceProvider ConfigureServices()
        {
            IServiceCollection serviceCollection = new ServiceCollection();

            serviceCollection.AddDbContext<EmployeesContext>(options => options.UseSqlServer(ConnectionConfig.ConnectionString));
            serviceCollection.AddAutoMapper(cfg => cfg.AddProfile<AutoMapperProfile>());

            serviceCollection.AddTransient<IEmployeeService, EmployeeService>();
            serviceCollection.AddTransient<ICommandParser, CommandParser>();
            serviceCollection.AddTransient<IEmployeeController, EmployeeController>();
            serviceCollection.AddTransient<IManagerController, ManagerController>();

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
            return serviceProvider;
        }
    }
}
