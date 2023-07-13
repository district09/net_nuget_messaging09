﻿using System;
using System.Threading.Tasks;
using Apache.NMS;
using Messaging09.Amqp.Config;
using Tracer = Messaging09.Amqp.Tracing.Tracer;

namespace Messaging09.Amqp.Plugins;

public class ErrorHandlingPlugin : MessagingPlugin
{
    private readonly MessageHandlingConfig _config;

    public ErrorHandlingPlugin(MessageHandlingConfig config)
    {
        _config = config;
    }

    public override async Task<MessageOutcome> HandleInbound<TMessageType>(IMessage message,
        MessageHandler<TMessageType> handler)
    {
        try
        {
            return await base.HandleInbound(message, handler);
        }
        catch (Exception e)
        {
            Tracer.ErrorFormat("An exception occured while handling message: {0}", e.ToString());
            return _config.GetOutcomeForException(e);
        }
    }
}