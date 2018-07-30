namespace AutoMapping.Core.Commands.Contracts
{
    internal interface ICommand
    {
        string Execute(params string[] args);
    }
}
