using System.Security.Claims;
using FastEndpoints;
using MediatR;
using Microsoft.EntityFrameworkCore.Internal;
using OpenIddict.Abstractions;
using TodoMinimal.API.Endpoints.NoteEndpoints.Requests;
using TodoMinimal.API.Endpoints.NoteEndpoints.Responses;
using TodoMinimal.API.Groups;
using TodoMinimal.API.Utils;
using TodoMinimal.Application.Common;
using TodoMinimal.Application.Notes;

namespace TodoMinimal.API.Endpoints.NoteEndpoints
{
    public sealed class GetAllNotesEndpoint(ISender sender) : Endpoint<GetNotesRequest, PagedResult<NoteResponse>>
    {
        public override void Configure()
        {
            Get("/all");
            Roles("Admin");
            Group<NoteGroup>();
            Summary(s =>
            {
                s.Summary = "Get paginated notes";
                s.Description = "Returns paged list of notes";
                s.Params["pageNumber"] = "Page Number (default is 1)";
                s.Params["pageSize"] = "Page Size (default is 25)";
            });
            Description(b => b
                .Produces<PagedResult<NoteResponse>>(200, "application/json"));
        }

        public override async Task HandleAsync(GetNotesRequest req, CancellationToken ct)
        {
            int pageNumber = Query<int?>("pageNumber") ?? 1;
            int pageSize = Query<int?>("pageSize") ?? 25;

            var query = new GetAllNotesQuery(pageNumber, pageSize);
            var result = await sender.Send(query, ct);

            var notes = result.Items.Select(note => note.ToResponse()).ToList();           

            await Send.OkAsync(new PagedResult<NoteResponse>(notes, result.PageData), ct);
        }
    }
}