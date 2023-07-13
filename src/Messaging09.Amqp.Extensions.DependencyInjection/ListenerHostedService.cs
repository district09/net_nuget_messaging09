using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Messaging09.Amqp.Extensions.DependencyInjection;

internal class ListenerHostedService : BackgroundService
{
    private readonly ILogger<ListenerHostedService> _logger;
    private readonly IListener _listener;
    private readonly string _fqdn;

    public ListenerHostedService(ILogger<ListenerHostedService> logger,
        IListener listener,
        string fqdn)
    {
        _logger = logger;
        _listener = listener;
        _fqdn = fqdn;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await StartListener();
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Unexpected error occurred");

            // Perhaps the host application should shut down now, but it might be possible that the error is recoverable
        }
    }

    private async Task StartListener()
    {
        _logger.LogInformation("Starting hosted service listener for {Fqdn}", _fqdn);
        await _listener.StartListening(_fqdn);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("hosted service for {Fqdn} is stopping", _fqdn);
        await _listener.StopListening();
        await base.StopAsync(cancellationToken);
    }
}