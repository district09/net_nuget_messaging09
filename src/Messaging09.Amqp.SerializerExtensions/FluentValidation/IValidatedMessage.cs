using FluentValidation;

namespace Messaging09.Amqp.SerializerExtensions.FluentValidation;

public interface IValidatedMessage<TMessageType>
{
  /// <summary>
  /// Adds validation rules to the message type using FluentValidation.
  /// </summary>
  /// <param name="builder">The abstract validator to add validation rules to.</param>
  void AddValidationRules(AbstractValidator<TMessageType> builder);
}
