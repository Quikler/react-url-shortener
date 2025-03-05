namespace WebApi.Contracts.V1.Responses;

public class PaginationResponse<T>
{
    public required int TotalCount { get; set; }
    public required int TotalPages { get; set; }
    public required int PageNumber { get; set; }
    public required int PageSize { get; set; }
    public required IEnumerable<T> Items { get; set; } = [];
}