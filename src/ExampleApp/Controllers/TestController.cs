using ExampleApp.Models;
using Messaging09.Amqp;
using Messaging09.Amqp.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;

namespace ExampleApp.Controllers;

[ApiController]
public class TestController : ControllerBase
{
    private readonly ILogger<TestController> _logger;
    private readonly IMessagePublisher _messagePublisher;
    private readonly CorrelationContextAccessor _contextAccessor;

    public TestController(ILogger<TestController> logger, IMessagePublisher messagePublisher,
        CorrelationContextAccessor contextAccessor)
    {
        _logger = logger;
        _messagePublisher = messagePublisher;
        _contextAccessor = contextAccessor;
    }

    [HttpGet("test")]
    public async Task<IActionResult> SendSingleMessage()
    {
        _contextAccessor.CorrelationId = Guid.Empty.ToString("B");
        var msg = new MessageViewModel() { Count = 1, Description = "Hello World111" };
        _logger.LogInformation("sending message");
        await _messagePublisher.SendMessage(msg, "topic://VirtualTopic.some.topic",
            message => message.Properties.SetString("Hello", "World"));
        return Ok("OK");
    }

    [HttpGet("bulk")]
    public async Task<IActionResult> SendMultipleMessages()
    {

        for (var i = 0; i < 1000; i++)
        {
            var msg = new MessageViewModel() { Count = i, Description = $"This is message number {i}" };
            await _messagePublisher.SendMessage(msg, "some.queue",
                message => message.Properties.SetString("Hello", "World"));
        }

        return Ok("OK");
    }
}
