using TodoMinimal.Application.Contracts;
using TodoMinimal.Application.Exceptions;
using TodoMinimal.Application.Repositories;
using TodoMinimal.Domain.Notes;
using TodoMinimal.Domain.SeedWork;

namespace TodoMinimal.Application.Notes
{
    public sealed class EditNoteCommand : CommandBase
    {
        public int NoteId { get; }
        public Guid UserId { get; set; }
        public string Content { get; }

        public EditNoteCommand(int noteId, Guid userId, string content)
        {
            NoteId = noteId;
            UserId = userId;
            Content = content;
        }
    }

    public class EditNoteCommandHander : ICommandHandler<EditNoteCommand>
    {
        private readonly INoteRepository _noteRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EditNoteCommandHander(INoteRepository noteRepository, IUnitOfWork unitOfWork)
        {
            _noteRepository = noteRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(EditNoteCommand request, CancellationToken cancellationToken)
        {
            var note = await _noteRepository.GetNote(request.NoteId, cancellationToken);
            if (note is null)
            {
                throw new NoteNotFoundException(request.NoteId);
            }

            if(note.UserId != request.UserId)
            {
                throw new UnauthorizedAccessException("You do not have permission to edit this note.");
            }
            
            note.UpdateContent(request.Content);

            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}