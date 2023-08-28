using Messaging09.Amqp.Tracing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Messaging09.Amqp.Extensions.DependencyInjection;

public class LogStartupFilter : IStartupFilter
{
    private readonly ILogger<ITrace> _logger;

    public LogStartupFilter(ILogger<ITrace> logger)
    {
        _logger = logger;
    }

    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return builder =>
        {
            Tracer.Trace = new LogTracer(_logger);
            next(builder);
        };
    }
}
