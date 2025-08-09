using FastEndpoints;
using MediatR;
using TodoMinimal.API.Groups;
using TodoMinimal.Application.Common;
using TodoMinimal.Application.Notes;
using OpenIddict.Abstractions;
using System.Security.Claims;
using TodoMinimal.API.Utils;
using TodoMinimal.API.Endpoints.NoteEndpoints.Responses;
using TodoMinimal.API.Endpoints.NoteEndpoints.Requests;

namespace TodoMinimal.API.Endpoints.NoteEndpoints
{
    public sealed class GetNotesEndpoint(ISender sender) : Endpoint<GetNotesRequest, PagedResult<NoteResponse>>
    {
        public override void Configure()
        {
            Get("/");
            // Version(2);
            Group<NoteGroup>();
            Policies("Authenticated");
            Summary(s =>
            {
                s.Summary = "Get paginated notes";
                s.Description = "Returns paged list of notes";
            });
            Description(b => b
                .Produces<PagedResult<NoteResponse>>(200, "application/json")
                .ProducesProblemDetails(404, "application/json+problem"));
        }

        public override async Task HandleAsync(GetNotesRequest req, CancellationToken ct)
        {
            var userId = Guid.Parse(User.FindFirstValue(OpenIddictConstants.Claims.Subject)!);
            
            var query = new GetNotesQuery(userId, req.PageNumber, req.PageSize);
            var result = await sender.Send(query, ct);

            var notes = result.Items.Select(note => note.ToResponse()).ToList();           

            await Send.OkAsync(new PagedResult<NoteResponse>(notes, result.PageData), ct);
        }
    }
}