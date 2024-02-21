using FluentValidation;
using Messaging09.Amqp.SerializerExtensions.FluentValidation;

namespace ValidationExample;

public class ValidatedMessage : IValidatedMessage<ValidatedMessage>
{
  public string Hello { get; set; }

  public void AddValidationRules(AbstractValidator<ValidatedMessage> validator)
  {
    validator.RuleFor(e => e.Hello).NotEmpty().NotEqual("123");
  }
}
