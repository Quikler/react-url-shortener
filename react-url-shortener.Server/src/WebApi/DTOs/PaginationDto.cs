namespace WebApi.DTOs;

public class PaginationDto<T>
{
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public IEnumerable<T> Items { get; set; } = [];

    public static PaginationDto<T> Empty => new();
}