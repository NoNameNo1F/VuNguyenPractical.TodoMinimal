using TodoMinimal.Application.Common;
using TodoMinimal.Application.Contracts;
using TodoMinimal.Application.Repositories;
using TodoMinimal.Domain.Notes;

namespace TodoMinimal.Application.Notes
{
    public sealed class GetAllNotesQuery(int pageNumber, int pageSize) : IQuery<PagedResult<Note>>
    {
        public int PageNumber { get; } = pageNumber;
        public int PageSize { get; } = pageSize;
    }

    public class GetAllNotesQueryHandler(INoteRepository noteRepository) : IQueryHandler<GetNotesQuery, PagedResult<Note>>
    {
        private readonly INoteRepository _noteRepository = noteRepository;

        public async Task<PagedResult<Note>> Handle(GetNotesQuery request, CancellationToken cancellationToken)
        {
            return await _noteRepository.GetAllNotes(request.PageNumber, request.PageSize, cancellationToken);
        }
    }
}