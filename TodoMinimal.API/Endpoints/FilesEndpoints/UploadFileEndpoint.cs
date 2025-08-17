using FastEndpoints;
using MediatR;
using TodoMinimal.API.Endpoints.FilesEndpoints.Requests;
using TodoMinimal.Application.Files;

namespace TodoMinimal.API.Endpoints.FilesEndpoints
{
    public class UploadFileEndpoint : Endpoint<FileUploadRequest, int>
    {
        private readonly ISender _sender;

        public UploadFileEndpoint(ISender sender)
        {
            _sender = sender;
        }

        public override void Configure()
        {
            Post("/files/upload");
            AllowFileUploads();
            Summary(s =>
            {
                s.Summary = "Upload Files";
                s.Description = "Upload multiple file with .txt, .xlsx, .csv exten";
            });
            Description(b => b
                .Accepts<FileUploadRequest>("multipart/form-data")
                .Produces<int>(201, "application/json")
                .ProducesProblemDetails(400, "application/json+problem"));
        }

        public override async Task HandleAsync(FileUploadRequest request, CancellationToken ct)
        {
            var command = new UploadFileCommand(request.Files.ToArray());
            await _sender.Send(command, ct);

            await Send.NoContentAsync(ct);
        }
    }
}