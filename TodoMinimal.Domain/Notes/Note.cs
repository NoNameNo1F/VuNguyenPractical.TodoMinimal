namespace TodoMinimal.Domain.Notes
{
    public class Note
    {
        public int Id { get; set; }
        public string Content { get; private set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? ModifiedAt { get; private set; }

        public static Note CreateNew(string content, Guid userId)
        {
            return new Note(content, userId);
        }

        private Note(string content, Guid userId)
        {
            UserId = userId;
            Content = content;
            CreatedAt = DateTime.Now;
        }

        public void UpdateContent(string content)
        {
            Content = content;
            ModifiedAt = DateTime.Now;
        }
    }
}