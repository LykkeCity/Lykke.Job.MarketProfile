using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Job.MarketProfile.Settings.JobSettings
{
    [UsedImplicitly]
    public class MyNoSqlSettings
    {
        public string WriterServiceUrl { get; set; }

        [Optional]
        public int MaxClientsInCache { get; set; } = 10000;
    }
}
