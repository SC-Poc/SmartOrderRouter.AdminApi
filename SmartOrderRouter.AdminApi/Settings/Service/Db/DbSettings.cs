using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace SmartOrderRouter.AdminApi.Settings.Service.Db
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnString { get; set; }
    }
}