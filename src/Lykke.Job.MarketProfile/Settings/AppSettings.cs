using Antares.Sdk.Settings;
using JetBrains.Annotations;
using Lykke.Job.MarketProfile.Settings.JobSettings;
namespace Lykke.Job.MarketProfile.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public MarketProfileJobSettings MarketProfileJob { get; set; }
    }
}
