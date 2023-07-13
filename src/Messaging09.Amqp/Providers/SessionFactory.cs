﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Apache.NMS;
using Apache.NMS.AMQP;
using Apache.NMS.AMQP.Meta;
using Messaging09.Amqp.Config;
using Messaging09.Amqp.Exceptions;

namespace Messaging09.Amqp.Providers
{
    public class SessionFactory : ISessionFactory, IDisposable
    {
        private readonly BrokerOptions _options;
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
        private ISession? _session;
        private readonly int _prefetchPolicy;

        public SessionFactory(BrokerOptions options, MessageHandlingConfig messageHandlingConfig)
        {
            _options = options;
            _prefetchPolicy = messageHandlingConfig.PrefetchPolicy;
        }

        public async Task<ISession> GetSession()
        {
            if (_session != null)
            {
                return _session;
            }

            await _semaphoreSlim.WaitAsync();
            try
            {
                _session = await CreateSession();
            }
            finally
            {
                _semaphoreSlim.Release();
            }

            return _session;
        }

        private async Task<ISession> CreateSession()
        {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(_options.TimeoutSeconds));
            var unregisterTOken =
                cts.Token.Register(() => throw new Exception("could not connect to amq in 20 seconds"));
            var connectionFactory = new ConnectionFactory(_options.Uri)
            {
                PrefetchPolicy = new PrefetchPolicyInfo()
                {
                    All = _prefetchPolicy,
                    QueuePrefetch = _prefetchPolicy,
                    TopicPrefetch = _prefetchPolicy,
                    DurableTopicPrefetch = _prefetchPolicy,
                    QueueBrowserPrefetch = _prefetchPolicy
                }
            };
            var connection =
                await connectionFactory.CreateConnectionAsync(_options.Username,
                    _options.Password);
            await connection.StartAsync();
            var session = await connection.CreateSessionAsync(AcknowledgementMode.IndividualAcknowledge);
            unregisterTOken.Unregister();
            return session;
        }

        public void Dispose()
        {
            _semaphoreSlim.Dispose();
            _session?.Dispose();
        }
    }
}