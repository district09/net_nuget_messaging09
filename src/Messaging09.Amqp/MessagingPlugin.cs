using Apache.NMS;

namespace Messaging09.Amqp;

public abstract class MessagingPlugin
{
    private MessagingPlugin? _next;

    public void SetNext(MessagingPlugin next)
    {
        if (_next != null)
        {
            _next.SetNext(next);
        }
        else
        {
            _next = next;
        }
    }

    public virtual async Task<MessageOutcome> HandleInbound<TMessageType>(
        IMessage message,
        MessageHandler<TMessageType> handler)
        where TMessageType : class
    {
        if (_next == null)
        {
            return await handler.Handle(message);
        }

        return await _next.HandleInbound(message, handler);
    }

    public virtual async Task<IMessage> HandleOutbound(IMessage message, IDestination fqdn)
    {
        if (_next == null)
        {
            return message;
        }

        return await _next.HandleOutbound(message, fqdn);
    }
}
