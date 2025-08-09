using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TodoMinimal.Application.Exceptions;

namespace TodoMinimal.API.Extensions;
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;

    public GlobalExceptionHandler(IProblemDetailsService problemDetailsService)
    {
        _problemDetailsService = problemDetailsService;
    }
    
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        ProblemDetails problemDetails = new ProblemDetails();
        
        switch (exception)
        {
            case NoteContentEmptyException:
                problemDetails = new ProblemDetails
                {
                    Title = "Note Content is required",
                    Detail = exception.Message,
                    Status = StatusCodes.Status400BadRequest,
                    Type = nameof(NoteContentEmptyException),
                };
                break;
            
            case NoteNotFoundException:
                problemDetails = new ProblemDetails
                {
                    Title = "Note was Not Found",
                    Detail = exception.Message,
                    Status = StatusCodes.Status404NotFound,
                    Type = nameof(NoteNotFoundException)
                };
                break;
                
            // case UnauthorizedAccessException:
            //     problemDetails = new ProblemDetails
            //     {
            //         Title = "Unauthorized Access",
            //         Detail = exception.Message,
            //         Status = StatusCodes.Status403Forbidden,
            //         Type = nameof(UnauthorizedAccessException)
            //     };

            default:
                problemDetails = new ProblemDetails
                {
                    Title = "An error occurred",
                    Detail = exception.Message,
                    Status = StatusCodes.Status500InternalServerError,
                    Type = exception.GetType().Name
                };
                break;
        }

        httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
    
        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails,
            Exception = exception
        });
    }
}