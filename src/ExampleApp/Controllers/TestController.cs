using ExampleApp.Models;
using Messaging09.Amqp;
using Microsoft.AspNetCore.Mvc;

namespace ExampleApp.Controllers;

[ApiController]
public class TestController : ControllerBase
{
    private readonly ILogger<TestController> _logger;
    private readonly IMessagePublisher<MessageViewModel> _messagePublisher;

    public TestController(ILogger<TestController> logger, IMessagePublisher<MessageViewModel> messagePublisher)
    {
        _logger = logger;
        _messagePublisher = messagePublisher;
    }

    [HttpGet("test")]
    public async Task<IActionResult> SendSingleMessage()
    {
        _logger.LogInformation("received send command");
        var msg = new MessageViewModel() { Count = 1, Description = "Hello World111" };
        await _messagePublisher.SendMessage(msg, "some.queue",
            message => message.Properties.SetString("Hello", "World"));
        _logger.LogInformation("message sent");
        return Ok("OK");
    }
}