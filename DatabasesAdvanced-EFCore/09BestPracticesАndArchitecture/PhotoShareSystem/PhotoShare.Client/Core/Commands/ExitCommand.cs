namespace PhotoShare.Client.Core.Commands
{
    using System;

    using Contracts;

    public class ExitCommand : ICommand
    {
        public string Execute(string[] data)
        {
            Console.WriteLine("Bye-bye!");
            Environment.Exit(0);
            return null;
        }
    }
}
