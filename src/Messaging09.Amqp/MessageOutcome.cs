namespace Messaging09.Amqp;

public enum MessageOutcome
{
    Ack,
    Failed,
    FailedUndeliverable,
    Reject
}
