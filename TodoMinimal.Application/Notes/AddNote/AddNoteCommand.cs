using TodoMinimal.Application.Contracts;
using TodoMinimal.Application.Exceptions;
using TodoMinimal.Application.Repositories;
using TodoMinimal.Domain.Notes;
using TodoMinimal.Domain.SeedWork;

namespace TodoMinimal.Application.Notes
{
    public sealed class AddNoteCommand(string content, Guid userId) : CommandBase<Note>
    {
        public string Content { get; } = content;
        public Guid UserId { get; set; } = userId;
    }

    public class AddNoteCommandHandler(INoteRepository noteRepository, IUnitOfWork unitOfWork) : ICommandHandler<AddNoteCommand, Note>
    {
        private readonly INoteRepository _noteRepository = noteRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Note> Handle(AddNoteCommand request, CancellationToken cancellationToken)
        {
            if (request.Content is null)
            {
                throw new NoteContentEmptyException();
            }

            Note note = Note.CreateNew(request.Content, request.UserId);
            await _noteRepository.AddNote(note, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            
            return note;
        }
    }
}