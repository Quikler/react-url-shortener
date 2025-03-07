namespace WebApi.DTOs;

public class PaginationDto<T>
{
    public required int TotalCount { get; set; }
    public required int TotalPages { get; set; }
    public required int PageNumber { get; set; }
    public required int PageSize { get; set; }
    public required IEnumerable<T> Items { get; set; } = [];

    public static PaginationDto<T> Empty => new()
    {
        TotalCount = 0,
        TotalPages = 1,
        PageNumber = 1,
        PageSize = 1,
        Items = [],
    };
}