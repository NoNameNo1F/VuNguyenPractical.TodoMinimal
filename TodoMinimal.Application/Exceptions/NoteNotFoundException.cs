using TodoMinimal.Domain.Notes;

namespace TodoMinimal.Application.Exceptions
{
    public class NoteNotFoundException : Exception
    {
        public NoteNotFoundException(int noteId) : base($"Note with Id {noteId} does not exist")
        {
        }
    }
}