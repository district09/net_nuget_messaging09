using System.Diagnostics;
using Apache.NMS;

namespace Messaging09.Amqp.Tracing;

public class TracingPlugin : MessagingPlugin
{
    // ReSharper disable InconsistentNaming
    private const string PARENT_TRACE_ID_KEY = "MSG_PARENT_TRACE_ID";
    private const string PARENT_SPAN_ID_KEY = "MSG_PARENT_SPAN_ID";
    // ReSharper enable once InconsistentNaming

    private readonly ActivitySource _actSource = new("Messaging");

    public override async Task<MessageOutcome> HandleInbound<TMessageType>(IMessage message,
        MessageHandler<TMessageType> handler)
    {
        var parentTraceId = message.Properties.Contains(PARENT_TRACE_ID_KEY)
            ? message.Properties.GetString(PARENT_TRACE_ID_KEY)
            : "";
        var parentSpanId = message.Properties.Contains(PARENT_SPAN_ID_KEY)
            ? message.Properties.GetString(PARENT_SPAN_ID_KEY)
            : "";

        ActivityContext parent;

        if (!string.IsNullOrWhiteSpace(parentTraceId) && !string.IsNullOrWhiteSpace(parentSpanId))
        {
            parent = new ActivityContext(ActivityTraceId.CreateFromString(parentTraceId),
                ActivitySpanId.CreateFromString(parentSpanId), ActivityTraceFlags.Recorded, null, true);
        }

        using var activity = _actSource.StartActivity(
            $"Message received: {handler.GetType().Name}",
            ActivityKind.Consumer,
            parent,
            new KeyValuePair<string, object?>[]
            {
                new("messaging.system", "Amqp"),
                new("messaging.destination", message.NMSDestination.ToString())
            });

        var outcome = await base.HandleInbound(message, handler);
        return outcome;
    }

    public override async Task<IMessage> HandleOutbound(IMessage message, IDestination fqdn)
    {
        using var activity = _actSource.StartActivity($"Send message to {fqdn}", ActivityKind.Producer);

        message.Properties.SetString(PARENT_SPAN_ID_KEY, activity?.SpanId.ToHexString());
        message.Properties.SetString(PARENT_TRACE_ID_KEY, activity?.TraceId.ToHexString());

        return await base.HandleOutbound(message, fqdn);
    }
}