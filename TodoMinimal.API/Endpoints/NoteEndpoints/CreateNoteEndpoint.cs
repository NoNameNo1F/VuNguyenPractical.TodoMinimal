using System.Security.Claims;
using FastEndpoints;
using MediatR;
using OpenIddict.Abstractions;
using TodoMinimal.API.Endpoints.NoteEndpoints.Requests;
using TodoMinimal.API.Endpoints.NoteEndpoints.Responses;
using TodoMinimal.API.Groups;
using TodoMinimal.API.Utils;
using TodoMinimal.Application.Notes;

namespace TodoMinimal.API.Endpoints.NoteEndpoints
{
    public class CreateNoteEndpoint(ISender sender) : Endpoint<CreateNoteRequest, NoteResponse>
    {
        public override void Configure()
        {
            Post("/create");
            Group<NoteGroup>();
            Policies("Authenticated");
            Summary(s =>
            {
                s.Summary = "Create a new note";
                s.Description = "Adds a new note with provided content";
            });
            Description(b => b
                .Accepts<CreateNoteRequest>("application/json")
                .Produces<NoteResponse>(201, "application/json")
                .ProducesProblemDetails(400, "application/json+problem"));
            //.ProducesProblemFE(400) //shortcut for .Produces<ErrorResponse>(400)
        }

        public override async Task HandleAsync(CreateNoteRequest request, CancellationToken ct)
        {
            var userId = Guid.Parse(User.FindFirstValue(OpenIddictConstants.Claims.Subject)!);
            var command = new AddNoteCommand(request.Content, userId);

            var newNote = await sender.Send(command, ct);
            
            var response = newNote.ToResponse();

            await Send.CreatedAtAsync<GetNoteEndpoint>(
                new { noteId = response.Id },
                response,
                cancellation: ct);
        }
    }
}