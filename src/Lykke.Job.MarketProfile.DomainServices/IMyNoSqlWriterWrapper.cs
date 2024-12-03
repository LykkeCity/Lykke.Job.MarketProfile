using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MyNoSqlServer.Abstractions;

namespace Lykke.Job.MarketProfile.DomainServices
{
    public interface IMyNoSqlWriterWrapper<TEntity> where TEntity : IMyNoSqlDbEntity, new()
    {
        Task<bool> TryInsertOrReplaceAsync(TEntity entity);
        Task<bool> TryDeleteAsync(string partitionKey, string rowKey);
        void Start(Func<IList<TEntity>> readAllRecordsCallback, TimeSpan? reloadTimerPeriod = null);
        void StartWithClearing(int countInCache, TimeSpan? reloadTimerPeriod = null);
    }
}
