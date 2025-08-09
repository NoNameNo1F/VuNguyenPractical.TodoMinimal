namespace TodoMinimal.Domain.Notes
{
    public record NoteId
    {
        public int Value { get; }
        public NoteId(int value)
        {
            Value = value;
        }
    }
}