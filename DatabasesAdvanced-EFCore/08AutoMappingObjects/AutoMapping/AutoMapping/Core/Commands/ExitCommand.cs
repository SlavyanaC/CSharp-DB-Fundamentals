namespace AutoMapping.Core.Commands
{
    using System;
    using AutoMapping.Core.Commands.Contracts;

    internal class ExitCommand : ICommand
    {
        public string Execute(params string[] args)
        {
            Console.WriteLine("Goodbye!");
            Environment.Exit(0);
            return string.Empty;
        }
    }
}
