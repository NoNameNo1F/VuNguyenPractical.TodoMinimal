namespace TodoMinimal.Application.Exceptions
{
    public class NoteContentEmptyException : Exception
    {
        public NoteContentEmptyException() : base($"Content is empty.")
        {
        }
    }
}