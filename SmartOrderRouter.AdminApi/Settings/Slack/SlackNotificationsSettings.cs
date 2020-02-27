using JetBrains.Annotations;

namespace SmartOrderRouter.AdminApi.Settings.Slack
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class SlackNotificationsSettings
    {
        public AzureQueueSettings AzureQueue { get; set; }
    }
}
