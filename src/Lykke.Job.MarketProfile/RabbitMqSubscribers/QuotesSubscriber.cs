using System;
using System.Threading.Tasks;
using Autofac;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Job.MarketProfile.Domain.Services;
using Lykke.Job.QuotesProducer.Contract;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;

namespace Lykke.Job.MarketProfile.RabbitMqSubscribers
{
    [UsedImplicitly]
    public class QuotesSubscriber : IStartable, IDisposable
    {
        private readonly string _connectionString;
        private readonly string _exchangeName;
        private readonly string _queueSuffix;
        private readonly IAssetPairsCacheService _cacheService;
        private readonly ILogFactory _logFactory;
        private readonly ILog _log;
        private RabbitMqSubscriber<QuoteMessage> _subscriber;

        public QuotesSubscriber(
            string connectionString,
            string exchangeName,
            string queueSuffix,
            IAssetPairsCacheService cacheService,
            ILogFactory logFactory)
        {
            _connectionString = connectionString;
            _exchangeName = exchangeName;
            _queueSuffix = queueSuffix;
            _cacheService = cacheService;
            _logFactory = logFactory;
            _log = logFactory.CreateLog(this);
        }
        
        public void Start()
        {
            try
            {
                var settings = RabbitMqSubscriptionSettings
                    .ForSubscriber(_connectionString, "lykke", _exchangeName, "lykke", $"marketprofilejob{_queueSuffix}");

                _subscriber = new RabbitMqSubscriber<QuoteMessage>(_logFactory, settings,
                        new ResilientErrorHandlingStrategy(_logFactory, settings,
                            retryTimeout: TimeSpan.FromSeconds(10),
                            retryNum: 10,
                            next: new DeadQueueErrorHandlingStrategy(_logFactory, settings)))
                    .SetMessageDeserializer(new JsonMessageDeserializer<QuoteMessage>())
                    .SetMessageReadStrategy(new MessageReadQueueStrategy())
                    .Subscribe(ProcessQuote)
                    .CreateDefaultBinding()
                    .Start();
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                throw;
            }
        }

        private async Task ProcessQuote(QuoteMessage entry)
        {
            try
            {
                await _cacheService.UpdatePairAsync(entry);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
        }

        public void Dispose()
        {
            _subscriber?.Stop();
        }
    }
}
