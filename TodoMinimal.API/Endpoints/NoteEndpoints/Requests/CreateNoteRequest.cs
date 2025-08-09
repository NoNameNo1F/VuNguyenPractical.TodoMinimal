using FastEndpoints;

namespace TodoMinimal.API.Endpoints.NoteEndpoints.Requests;
public class CreateNoteRequest
{
    [FormField]
    public string Content { get; set; } = string.Empty;
}