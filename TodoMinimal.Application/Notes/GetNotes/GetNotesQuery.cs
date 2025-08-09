using TodoMinimal.Application.Common;
using TodoMinimal.Application.Contracts;
using TodoMinimal.Application.Repositories;
using TodoMinimal.Domain.Notes;

namespace TodoMinimal.Application.Notes
{
    public sealed class GetNotesQuery(Guid userId, int pageNumber, int pageSize) : IQuery<PagedResult<Note>>
    {
        public Guid UserId { get; set; } = userId;
        public int PageNumber { get; } = pageNumber;
        public int PageSize { get; } = pageSize;
    }

    public class GetNotesQueryHandler(INoteRepository noteRepository) : IQueryHandler<GetNotesQuery, PagedResult<Note>>
    {
        private readonly INoteRepository _noteRepository = noteRepository;

        public async Task<PagedResult<Note>> Handle(GetNotesQuery request, CancellationToken cancellationToken)
        {
            return await _noteRepository.GetNotesForUser(request.UserId, request.PageNumber, request.PageSize, cancellationToken);
        }
    }
}