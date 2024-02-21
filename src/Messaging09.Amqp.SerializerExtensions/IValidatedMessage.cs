using FluentValidation;

namespace Messaging09.Amqp.SerializerExtensions;

public interface IValidatedMessage<TMessageType>
{
  void AddValidationRules(AbstractValidator<TMessageType> builder);
}
