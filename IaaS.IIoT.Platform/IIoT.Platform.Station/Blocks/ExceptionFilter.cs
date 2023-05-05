namespace Platform.Station.Blocks;
internal sealed class ExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var result = new ValidationProblemDetails
        {
            Title = context.Exception.Message,
            Detail = context.Exception.StackTrace,
            Status = StatusCodes.Status500InternalServerError
        };
        context.Result = new ObjectResult(result)
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };
    }
}