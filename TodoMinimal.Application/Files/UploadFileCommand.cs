using Microsoft.AspNetCore.Http;
using TodoMinimal.Application.Contracts;

namespace TodoMinimal.Application.Files
{
    public sealed class UploadFileCommand : CommandBase
    {
        public IFormFile[] Files { get; }

        public UploadFileCommand(IFormFile[] files)
        {
            Files = files;
        }
    }

    public class UploadFileCommandHandler : ICommandHandler<UploadFileCommand>
    {
        public Task Handle(UploadFileCommand request, CancellationToken cancellationToken)
        {
            if(request.Files is null || !request.Files.Any())
            {
                throw new FileNotFoundException();
            }

            return Task.CompletedTask;
        }
    }
}