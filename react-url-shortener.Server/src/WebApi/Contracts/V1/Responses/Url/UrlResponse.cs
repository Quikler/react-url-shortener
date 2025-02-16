namespace WebApi.Contracts.V1.Responses.Url;

public class UrlResponse
{
    public required Guid Id { get; set; }

    public required string UrlOriginal { get; set; }
    public required string UrlShortened { get; set; }

    public required Guid UserId { get; set; }
}