using Apache.NMS;
using Xunit.Abstractions;

namespace Messaging09.Amqp.IntegrationTests;

public class NmsTestTracer(ITestOutputHelper logger) : ITrace
{
  public void Debug(string message)
  {
    logger.WriteLine(message);
  }

  public void Info(string message)
  {
    logger.WriteLine(message);
  }

  public void Warn(string message)
  {
    logger.WriteLine(message);
  }

  public void Error(string message)
  {
    logger.WriteLine(message);
  }

  public void Fatal(string message)
  {
    logger.WriteLine(message);
  }

  public bool IsDebugEnabled { get; set; } = true;

  public bool IsInfoEnabled => true;
  public bool IsWarnEnabled => true;
  public bool IsErrorEnabled => true;
  public bool IsFatalEnabled => true;
}
