namespace TodoMinimal.API.Endpoints.NoteEndpoints.Responses;
public class NoteResponse
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; } = null;
}