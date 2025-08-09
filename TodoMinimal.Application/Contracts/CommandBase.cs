namespace TodoMinimal.Application.Contracts
{
    public class CommandBase : ICommand
    {
    }

    public class CommandBase<TResult> : ICommand<TResult>
    {
    }
}