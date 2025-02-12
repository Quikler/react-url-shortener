namespace WebApi.Contracts.V1.Responses;

public class FailureResponse(IEnumerable<string> errors)
{
    public IEnumerable<string> Errors { get; set; } = errors;

    public FailureResponse(string error) : this([error]) { }
}