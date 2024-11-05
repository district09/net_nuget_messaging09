using System.ComponentModel.DataAnnotations;
using Apache.NMS.Policies;

namespace Messaging09.Amqp.Config;

public class BrokerOptions
{
  public const string Prefix = "Broker";
  [Required] public string Uri { get; set; }
  [Required] public string Username { get; set; }
  [Required] public string Password { get; set; }
  [NotNegative] public int TimeoutSeconds { get; set; } = 20;
  [NotNegative] public int PrefetchPolicy { get; set; } = 2;
  public RedeliveryPolicy RedeliveryPolicy { get; set; }
}

public class NotNegativeAttribute : ValidationAttribute
{
  public override string FormatErrorMessage(string name)
  {
    return $"The field {name} must be a non-negative number.";
  }

  public override bool IsValid(object? value)
  {
    return value is >= 0;
  }
}
