using System;
using JetBrains.Annotations;

namespace Lykke.Job.MarketProfile.Settings.JobSettings
{
    [UsedImplicitly]
    public class MarketProfileJobSettings
    {
        public DbSettings Db { get; set; }
        public BlobSettings Blob { get; set; }
        public RabbitMqSettings RabbitMq { get; set; }
        public RedisSettings Redis { get; set; }
        public TimeSpan PersistPeriod { get; set; }
        public MyNoSqlSettings MyNoSqlServer { get; set; }
    }
}
