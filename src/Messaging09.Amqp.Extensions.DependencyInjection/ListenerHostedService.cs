using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Messaging09.Amqp.Extensions.DependencyInjection;

internal class ListenerHostedService<TMessageType> : BackgroundService where TMessageType : class
{
    private readonly ILogger<ListenerHostedService<TMessageType>> _logger;
    private readonly ListenerFactory _listenerFactory;
    private readonly string _fqdn;
    private IListener? _listener;

    public ListenerHostedService(ILogger<ListenerHostedService<TMessageType>> logger,
        ListenerFactory listenerFactory,
        string fqdn)
    {
        _logger = logger;
        _listenerFactory = listenerFactory;
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
        _listener = _listenerFactory.GetListener<TMessageType>(_fqdn);
        await _listener.StartListening();
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("hosted service for {Fqdn} is stopping", _fqdn);
        if (_listener != null) await _listener.StopListening();
        await base.StopAsync(cancellationToken);
    }
}
