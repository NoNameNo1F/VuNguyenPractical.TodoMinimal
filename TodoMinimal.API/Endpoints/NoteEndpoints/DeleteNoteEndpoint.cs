using FastEndpoints;
using MediatR;
using TodoMinimal.API.Groups;
using System.Security.Claims;
using OpenIddict.Abstractions;
using Microsoft.AspNetCore.Http.HttpResults;
using TodoMinimal.API.Endpoints.NoteEndpoints.Requests;
using TodoMinimal.Application.Notes.DeleteNote;

namespace TodoMinimal.API.Endpoints.NoteEndpoints;
public class DeleteNoteEndpoint(ISender sender) : EndpointWithoutRequest<NoContent>
{
    public override void Configure()
    {
        Delete("/{noteId:int}");
        Group<NoteGroup>();
        Policies("Authenticated");
        Summary(s =>
        {
            s.Summary = "Delete a note note";
            s.Description = "Delete a note with provided noteId";
        });
        Description(b => b
                .Produces(204)
                .ProducesProblemFE(400)
                .ProducesProblemDetails(404, "application/json+problem"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var noteId = Route<int>("noteId");
        var currentUserId = Guid.Parse(User.FindFirstValue(OpenIddictConstants.Claims.Subject)!);
        var command = new DeleteNoteCommand(
            noteId,
            currentUserId);

        await sender.Send(command, ct);
        await Send.NoContentAsync(ct);
    }
}