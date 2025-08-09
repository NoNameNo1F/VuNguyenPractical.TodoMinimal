using FastEndpoints;

namespace TodoMinimal.API.Endpoints.Shared
{
    public class PagingRequestDto
    {
        [QueryParam, BindFrom("pageNumber")]
        // [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;
        
        [QueryParam, BindFrom("pageSize")]
        // [Range(1, int.MaxValue)]
        public int PageSize { get; set; } = 25;
    }
}