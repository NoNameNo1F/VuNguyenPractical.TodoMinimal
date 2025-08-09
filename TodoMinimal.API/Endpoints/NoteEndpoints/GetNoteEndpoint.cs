using System.Security.Claims;
using FastEndpoints;
using MediatR;
using OpenIddict.Abstractions;
using TodoMinimal.API.Endpoints.NoteEndpoints.Responses;
using TodoMinimal.API.Groups;
using TodoMinimal.API.Utils;
using TodoMinimal.Application.Notes.GetNote;
using TodoMinimal.Domain.Notes;

namespace TodoMinimal.API.Endpoints.NoteEndpoints
{
    public class GetNoteEndpoint(ISender sender) : EndpointWithoutRequest<NoteResponse>
    {
        public override void Configure()
        {
            Get("/{noteId:int}");
            Group<NoteGroup>();
            Policies("Authenticated");
            Summary(s =>
            {
                s.Summary = "Get a note";
                s.Description = "Get a note with provided id";
                s.Params["noteId"] = "the Id of Note";
            });
            Description(b => b
                .Produces<NoteResponse>(200, "application/json")
                .ProducesProblemDetails(404, "application/json+problem"));
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var noteId = Route<int>("noteId");
            var currentUserId = Guid.Parse(User.FindFirstValue(OpenIddictConstants.Claims.Subject)!);
            var query = new GetNoteQuery(noteId, currentUserId);
            var note = await sender.Send(query, ct);
            
            await Send.OkAsync(note!.ToResponse(), ct);
        }
    }
}