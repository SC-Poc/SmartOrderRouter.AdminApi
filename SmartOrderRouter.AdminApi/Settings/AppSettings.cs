using JetBrains.Annotations;
using Lykke.Service.SmartOrderRouter.Client;
using SmartOrderRouter.AdminApi.Settings.Clients;
using SmartOrderRouter.AdminApi.Settings.Service;
using SmartOrderRouter.AdminApi.Settings.Slack;

namespace SmartOrderRouter.AdminApi.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings
    {
        public AdminApiSettings AdminApi { get; set; }

        public SmartOrderRouterServiceClientSettings SmartOrderRouterServiceClient { get; set; }

        public SlackNotificationsSettings SlackNotifications { get; set; }

        public MonitoringServiceClientSettings MonitoringServiceClient { get; set; }
    }
}