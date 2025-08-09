using FastEndpoints;

namespace TodoMinimal.API.Endpoints.NoteEndpoints.Requests;
public class DeleteNoteRequest
{
    [RouteParam, BindFrom("nodeId")]
    public required int NoteId { get; set; }
}