using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Lykke.Job.MarketProfile.DomainServices;
using Lykke.Job.MarketProfile.NoSql.Models;
using Lykke.Job.MarketProfile.Settings;
using Lykke.SettingsReader;
using MyNoSqlServer.Abstractions;

namespace Lykke.Job.MarketProfile.Modules
{
    public class MyNoSqlModule : Module
    {
        private readonly string _myNoSqlServer;
        public MyNoSqlModule(IReloadingManager<AppSettings> settingsManager)
        {
            var settings = settingsManager.CurrentValue.MarketProfileJob.MyNoSqlServer;

            if (string.IsNullOrEmpty(settings?.WriterServiceUrl))
            {
                Console.WriteLine("Please fill in settings [MarketProfileJob.MyNoSqlServer.WriterServiceUrl]");

                throw new Exception("Please fill in settings MarketProfileJob.MyNoSqlServer.WriterServiceUrl");
            }

            _myNoSqlServer = settings.WriterServiceUrl;
        }

        protected override void Load(ContainerBuilder builder)
        {
            RegisterMyNoSqlWriter<AssetPairPriceNoSql>(builder, AssetPairPriceNoSql.TableName);
        }

        private void RegisterMyNoSqlWriter<TEntity>(ContainerBuilder builder, string table)
            where TEntity : IMyNoSqlDbEntity, new()
        {
            builder.Register(ctx =>
                {
                    return new MyNoSqlServer.DataWriter.MyNoSqlServerDataWriter<TEntity>(() => _myNoSqlServer, table);
                })
                .As<IMyNoSqlServerDataWriter<TEntity>>()
                .SingleInstance();

            builder.RegisterType<MyNoSqlWriterWrapper<TEntity>>()
                .As<IMyNoSqlWriterWrapper<TEntity>>()
                .SingleInstance();

        }
    }
}
