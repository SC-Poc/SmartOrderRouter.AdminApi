using Autofac;
using Lykke.Service.SmartOrderRouter.Client;
using SmartOrderRouter.AdminApi.Settings;

namespace SmartOrderRouter.AdminApi
{
    public class AutofacModule : Module
    {
        private readonly AppSettings _settings;

        public AutofacModule(AppSettings settings)
        {
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterSmartOrderRouterClient(_settings.SmartOrderRouterServiceClient, null);
        }
    }
}