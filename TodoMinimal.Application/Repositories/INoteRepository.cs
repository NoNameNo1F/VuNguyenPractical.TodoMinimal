using TodoMinimal.Application.Common;
using TodoMinimal.Domain.Notes;

namespace TodoMinimal.Application.Repositories
{
    public interface INoteRepository
    {
        Task<PagedResult<Note>> GetNotesForUser(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<PagedResult<Note>> GetAllNotes(int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<Note?> GetNote(int noteId, CancellationToken cancellationToken);
        Task AddNote(Note note, CancellationToken cancellationToken);
        Task RemoveNote(Note note);
    }
}