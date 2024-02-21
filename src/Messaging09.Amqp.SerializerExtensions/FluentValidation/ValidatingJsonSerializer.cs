using FluentValidation;
using Messaging09.Amqp.Serializers.Text;

namespace Messaging09.Amqp.SerializerExtensions.FluentValidation;

public class ValidatingJsonSerializer<TMessageType> : JsonTextMessageSerializer<TMessageType>
  where TMessageType : IValidatedMessage<TMessageType>
{
  protected override TMessageType DeserializeFromText(string text)
  {
    var result = base.DeserializeFromText(text);
    var validator = new MessageValidator();
    result.AddValidationRules(validator);
    validator.ValidateAndThrow(result);
    return result;
  }

  protected override string SerializeToText(TMessageType message)
  {
    return base.SerializeToText(message);
  }

  private class MessageValidator : AbstractValidator<TMessageType>
  {
    public MessageValidator()
    {
    }
  }
}
