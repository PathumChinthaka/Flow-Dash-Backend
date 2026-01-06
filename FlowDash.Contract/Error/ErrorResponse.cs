namespace FlowDash.Contract.Error
{
    public record ErrorResponse(
        int StatusCode,
        string Message,
        string TraceId
    );
}
