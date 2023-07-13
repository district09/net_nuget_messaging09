namespace Messaging09.Amqp.Config;

public class MessageHandlingConfig
{
    internal int PrefetchPolicy = 2;
    public MessageOutcome DefaultAck { get; set; } = MessageOutcome.Failed;
    public MessageOutcome UnhandledExceptionAck { get; set; } = MessageOutcome.FailedUndeliverable;

    private Dictionary<Type, MessageOutcome> AckTypeForExceptions { get; } = new();

    public void SetPrefetchPolicy(int concurrentMessages)
    {
        PrefetchPolicy = concurrentMessages;
    }

    public void AddKnownException<TExceptionType>(MessageOutcome outcome)
    {
        AckTypeForExceptions.Add(typeof(TExceptionType), outcome);
    }

    public MessageOutcome GetOutcomeForException<TExceptionType>(TExceptionType exception)
        where TExceptionType : Exception
    {
        var gotValue = AckTypeForExceptions.TryGetValue(exception.GetType(), out var outcome);
        return gotValue ? outcome : UnhandledExceptionAck;
    }
}