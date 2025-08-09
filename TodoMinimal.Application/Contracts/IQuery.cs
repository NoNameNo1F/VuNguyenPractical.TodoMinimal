using MediatR;

namespace TodoMinimal.Application.Contracts
{
    public interface IQuery<out TResult> : IRequest<TResult>
    {
    }
}