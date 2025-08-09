using TodoMinimal.Application.Contracts;
using TodoMinimal.Application.Exceptions;
using TodoMinimal.Application.Repositories;
using TodoMinimal.Domain.Notes;
using TodoMinimal.Domain.SeedWork;

namespace TodoMinimal.Application.Notes.DeleteNote
{
    public sealed class DeleteNoteCommand(int noteId, Guid userId) : CommandBase
    {
        public int NoteId { get; set; } = noteId;
        public Guid UserId { get; set; } = userId;
    }

    public class DeleteNoteCommandHander(INoteRepository noteRepository, IUnitOfWork unitOfWork) : ICommandHandler<DeleteNoteCommand>
    {
        private readonly INoteRepository _noteRepository = noteRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task Handle(DeleteNoteCommand request, CancellationToken cancellationToken)
        {
            Note note = await _noteRepository.GetNote(request.NoteId, cancellationToken) ?? throw new NoteNotFoundException(request.NoteId);
            if (note.UserId != request.UserId)
            {
                throw new UnauthorizedAccessException("You do not have permission to delete this note.");
            }

            await _noteRepository.RemoveNote(note);
            _ = await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}