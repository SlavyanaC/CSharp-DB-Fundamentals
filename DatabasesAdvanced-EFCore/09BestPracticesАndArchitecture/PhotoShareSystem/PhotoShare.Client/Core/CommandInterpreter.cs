namespace PhotoShare.Client.Core
{
    using System;
    using System.Linq;
    using System.Reflection;

    using Client.Core.Contracts;

    public class CommandInterpreter : ICommandInterpreter
    {
        private const string INVALID_COMMAND = "Command {0} not valid!";

        private readonly IServiceProvider serviceProvider;

        public CommandInterpreter(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public string Read(string[] input)
        {
            string inputCommand = input[0].ToLower() + "command";

            string[] args = input.Skip(1).ToArray();

            Type type = Assembly.GetCallingAssembly()
                               .GetTypes()
                               .FirstOrDefault(x => x.Name.ToLower() == inputCommand);

            if (type == null)
            {
                throw new InvalidOperationException(string.Format(INVALID_COMMAND, input[0]));
            }

            ConstructorInfo constructor = type.GetConstructors().First();

            Type[] constructorParameters = constructor.GetParameters().Select(x => x.ParameterType).ToArray();

            object[] service = constructorParameters.Select(serviceProvider.GetService).ToArray();

            ICommand command = (ICommand)constructor.Invoke(service);

            string result = command.Execute(args);
            return result;
        }
    }
}
