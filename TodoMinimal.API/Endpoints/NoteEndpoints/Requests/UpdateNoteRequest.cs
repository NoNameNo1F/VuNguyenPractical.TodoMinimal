using FastEndpoints;

namespace TodoMinimal.API.Endpoints.NoteEndpoints.Requests;

public class UpdateNoteRequest
{
    /// <summary>
    /// Id field description
    /// </summary>
    [FormField]
    public int Id { get; set; }
    
    /// <summary>
    /// Updated Content field description
    /// </summary>
    [FormField]
    public string Content { get; set; } = string.Empty;
}