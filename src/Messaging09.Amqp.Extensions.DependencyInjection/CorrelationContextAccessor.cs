namespace Messaging09.Amqp.Extensions.DependencyInjection;

public class CorrelationContextAccessor
{
    private string? _corrId;

    public string CorrelationId
    {
        get => string.IsNullOrWhiteSpace(_corrId) ? Guid.NewGuid().ToString("D") : _corrId;
        set => _corrId = value;
    }
}
