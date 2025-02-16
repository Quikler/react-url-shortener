namespace WebApi.Contracts.V1.Responses.Url;

public class UrlInfoResponse
{
    public required Guid Id { get; set; }

    public required string UrlOriginal { get; set; }
    public required string UrlShortened { get; set; }
    public required DateTime CreatedAt { get; set; }

    public required Guid UserId { get; set; }
}