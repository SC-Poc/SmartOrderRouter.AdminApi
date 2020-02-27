using JetBrains.Annotations;
using SmartOrderRouter.AdminApi.Settings.Service.Db;

namespace SmartOrderRouter.AdminApi.Settings.Service
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AdminApiSettings
    {
        public DbSettings Db { get; set; }
    }
}