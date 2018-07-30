namespace AutoMapping.Core
{
    using System;
    using System.Reflection;
    using System.Linq;
    using AutoMapping.Core.Commands.Contracts;
    using AutoMapping.Core.Contracts;

    class CommandParser : ICommandParser
    {
        private readonly IServiceProvider serviceProvider;

        public CommandParser(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public string Parse(string[] input)
        {
            string commandName = input[0].ToLower() + "command";

            string[] commandArgs = input.Skip(1).ToArray();

            Type commandType = Assembly.GetCallingAssembly()
                .GetTypes()
                .FirstOrDefault(x => x.Name.ToLower() == commandName);

            if (commandType == null)
            {
                throw new ArgumentException("Invalid command!");
            }

            var constructor = commandType.GetConstructors().First();

            var constructorParams = constructor.GetParameters()
                .Select(p => p.ParameterType)
                .ToArray();

            var service = constructorParams
                .Select(this.serviceProvider.GetService)
                .ToArray();

            ICommand command = (ICommand)constructor.Invoke(service);

            string result = command.Execute(commandArgs);

            return result;
        }
    }
}
