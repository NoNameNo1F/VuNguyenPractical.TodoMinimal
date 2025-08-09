using System.Security.Claims;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using OpenIddict.Abstractions;
using TodoMinimal.API.Endpoints.NoteEndpoints.Requests;
using TodoMinimal.API.Groups;
using TodoMinimal.Application.Notes;

namespace TodoMinimal.API.Endpoints.NoteEndpoints
{
    public class EditNoteEndpoint(ISender sender) : Endpoint<UpdateNoteRequest, NoContent>
    {
        public override void Configure()
        {
            Put("/{noteId:int}");
            Policies("Authenticated");
            Group<NoteGroup>();
            Summary(s =>
            {
                s.Summary = "Update a note";
                s.Description = "Updates a exists note with provided content, and id";
                s.Params["noteId"] = "the Id of Note";
            });
            Description(b => b
                .Accepts<UpdateNoteRequest>("application/json")
                .Produces(204)
                .ProducesProblemFE(400) //shortcut for .Produces<ErrorResponse>(400)
                .ProducesProblemDetails(404, "application/json+problem"));
        }

        public override async Task HandleAsync(UpdateNoteRequest req, CancellationToken ct)
        {
            var noteId = Route<int>("noteId");
            if(noteId != req.Id)
            {
                throw new ArgumentException("Route noteId does not match request Id.");
            }
            
            var userId = Guid.Parse(User.FindFirstValue(OpenIddictConstants.Claims.Subject)!);
            var command = new EditNoteCommand(req.Id, userId, req.Content);

            await sender.Send(command, ct);
            await Send.NoContentAsync(ct);
        }
    }
}