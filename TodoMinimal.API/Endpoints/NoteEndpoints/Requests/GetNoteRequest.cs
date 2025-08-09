using FastEndpoints;

namespace TodoMinimal.API.Endpoints.NoteEndpoints.Requests;

public class GetNoteRequest
{
    [RouteParam, BindFrom("nodeId")]
    public required int NoteId { get; set; }
}