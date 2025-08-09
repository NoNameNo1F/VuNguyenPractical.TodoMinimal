using TodoMinimal.API.Endpoints.NoteEndpoints.Responses;

namespace TodoMinimal.API.Utils;
public static class NoteMapper
{
    public static NoteResponse ToResponse(this Domain.Notes.Note note)
    {
        return new NoteResponse
        {
            Id = note.Id,
            UserId = note.UserId,
            Content = note.Content,
            CreatedAt = note.CreatedAt,
            UpdatedAt = note.ModifiedAt ?? null
        };
    }
}