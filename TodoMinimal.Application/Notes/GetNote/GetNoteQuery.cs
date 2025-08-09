using TodoMinimal.Application.Contracts;
using TodoMinimal.Application.Exceptions;
using TodoMinimal.Application.Repositories;
using TodoMinimal.Domain.Notes;

namespace TodoMinimal.Application.Notes.GetNote
{
    public sealed class GetNoteQuery(int noteId, Guid userId) : IQuery<Note?>
    {
        public int NoteId { get; } = noteId;
        public Guid UserId { get; set; } = userId;
    }

    public class GetNoteQueryHandler(INoteRepository noteRepository) : IQueryHandler<GetNoteQuery, Note?>
    {
        private readonly INoteRepository _noteRepository = noteRepository;

        public async Task<Note?> Handle(GetNoteQuery request, CancellationToken cancellationToken)
        {
            var note = await _noteRepository.GetNote(request.NoteId, cancellationToken) ?? throw new NoteNotFoundException(request.NoteId);
            if (note.UserId != request.UserId)
            {
                throw new UnauthorizedAccessException($"User {request.UserId} is not authorized to access note {request.NoteId}");
            }

            return note;
        }
    }
}