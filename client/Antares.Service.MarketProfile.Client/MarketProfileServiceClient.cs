using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Antares.Service.MarketProfile.LykkeClient;
using Lykke.Job.MarketProfile.Contract;
using Lykke.Job.MarketProfile.NoSql.Models;
using MyNoSqlServer.Abstractions;
using MyNoSqlServer.DataReader;
using LykkeMarketProfile = Antares.Service.MarketProfile.LykkeClient.LykkeMarketProfile;

namespace Antares.Service.MarketProfile.Client
{
    public class MarketProfileServiceClient : IMarketProfileServiceClient, IDisposable
    {
        private readonly MyNoSqlTcpClient _myNoSqlClient;

        private readonly object _locker = new object();
        private readonly IMyNoSqlServerDataReader<AssetPairPriceNoSql> _readerAssetPairNoSql;
        private readonly ILykkeMarketProfile _httpClient;
        private bool _isStarted = false;

        public MarketProfileServiceClient(
            string myNoSqlServerReaderHost,
            string marketServiceHttpApiUrl)
        {
            var host = Environment.GetEnvironmentVariable("HOST") ?? Environment.MachineName;
            _httpClient = new LykkeMarketProfile(new Uri(marketServiceHttpApiUrl));

            _myNoSqlClient = new MyNoSqlTcpClient(() => myNoSqlServerReaderHost, host);

            _readerAssetPairNoSql = new MyNoSqlReadRepository<AssetPairPriceNoSql>(_myNoSqlClient, AssetPairPriceNoSql.TableName);
        }

        public IMarketProfileServiceClient EventualCache => this;

        public ILykkeMarketProfile HttpClient => _httpClient;

        public void Start()
        {
            if (_isStarted)
                return;

            lock (_locker)
            {
                if (_isStarted)
                    return;

                StartPrivate();
            }
        }

        private void StartPrivate()
        {
            _myNoSqlClient.Start();

            var sw = new Stopwatch();
            sw.Start();

            int currentRetry = 0;
            var totalAmount = 1;

            for (;;)
            {
                try
                {
                    var allPairs = _httpClient.ApiMarketProfileGet();
                    totalAmount = allPairs.Count;
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    currentRetry++;

                    if (currentRetry > 10)
                    {
                        Console.WriteLine("Can not access market profile service to get amount of asset pairs");
                        break;
                    }
                }

                var delay = Math.Min((int) Math.Pow(2, currentRetry) * 100, 5000);
                Thread.Sleep(delay);
            }

            var isCacheSet = false;
            var iteration = 0;
            while (iteration < 100)
            {
                iteration++;
                if (EventualCache.GetAll().Count >= totalAmount)
                {
                    isCacheSet = true;
                    break;
                }

                Thread.Sleep(100);
            }

            sw.Stop();

            Console.WriteLine($"MarketProfileServiceClient client is started. Wait time: {sw.ElapsedMilliseconds} ms");

            if (!isCacheSet)
            {
                Console.WriteLine($"Cache items amount = {EventualCache.GetAll().Count}, " +
                                  $"while service items amount = {totalAmount}, They should be equal!");
            }

            _isStarted = true;
        }

        public void Dispose()
        {
            _myNoSqlClient.Stop();
        }

        IAssetPair IMarketProfileServiceClient.Get(string id)
        {
            try
            {
                var data = _readerAssetPairNoSql.Get(
                    AssetPairPriceNoSql.GeneratePartitionKey(),
                    AssetPairPriceNoSql.GenerateRowKey(id));

                return data?.AssetPair;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cannot read from MyNoSQL. Table: ${AssetPairPriceNoSql.TableName}, " +
                                                                               $"PK: {AssetPairPriceNoSql.GeneratePartitionKey()}, " +
                                                                               $"RK: {AssetPairPriceNoSql.GenerateRowKey(id)}, Ex: {ex}");
                throw;
            }
        }

        List<IAssetPair> IMarketProfileServiceClient.GetAll()
        {
            try
            {
                var data = _readerAssetPairNoSql.Get();

                return data.Select(e => (IAssetPair)e.AssetPair).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cannot read from MyNoSQL. Table: ${AssetPairPriceNoSql.TableName}, {ex}");
                throw;
            }
        }
    }
}
