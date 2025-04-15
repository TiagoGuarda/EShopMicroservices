using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BuildingBlocks.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IRequest<TResponse>
    where TResponse : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger) => _logger = logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[START] Request: {Request} | Response: {Response} | RequestData: {RequestData}", typeof(TRequest).Name, typeof(TResponse).Name, request);

        var timer = new Stopwatch();
        timer.Start();

        var response = await next();

        timer.Stop();
        var timeTaken = timer.Elapsed;
        if (timeTaken.Seconds > 3) //if the request is greater than 3 seconds, then will log the performance warning
            _logger.LogWarning("[PERFORMANCE] Request: {Request} | Response: {Response} | RequestData: {RequestData} | TimeTaken: {TimeTaken}", typeof(TRequest).Name, typeof(TResponse).Name, request, timeTaken);
                
        _logger.LogInformation("[END] Request: {Request} | Response: {Response} | TimeTaken: {TimeTaken}", typeof(TRequest).Name, typeof(TResponse).Name, timeTaken);

        return response;
    }
}
