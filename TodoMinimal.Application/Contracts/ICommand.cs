using MediatR;

namespace TodoMinimal.Application.Contracts
{
    public interface ICommand : IRequest
    {
    }

    public interface ICommand<TResult> : IRequest<TResult>
    {
    }
}