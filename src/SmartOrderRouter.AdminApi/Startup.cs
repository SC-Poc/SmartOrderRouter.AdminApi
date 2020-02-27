using System;
using System.Buffers;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using FluentValidation.AspNetCore;
using JetBrains.Annotations;
using Lykke.Common;
using Lykke.Common.ApiLibrary.Filters;
using Lykke.Common.ApiLibrary.Middleware;
using Lykke.Common.ApiLibrary.Swagger;
using Lykke.Common.Log;
using Lykke.Logs;
using Lykke.MonitoringServiceApiCaller;
using Lykke.SettingsReader;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartOrderRouter.AdminApi.Settings;

namespace SmartOrderRouter.AdminApi
{
    public class Startup
    {
        private const string ApiVersion = "v1";
        private const string ApiTitle = "Admin API";

        private AppSettings _appSettings;

        private IContainer ApplicationContainer { get; set; }
        private IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        [UsedImplicitly]
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var settingsManager = Configuration.LoadSettings<AppSettings>(options =>
            {
                options.SetConnString(x => x.SlackNotifications.AzureQueue.ConnectionString);
                options.SetQueueName(x => x.SlackNotifications.AzureQueue.QueueName);
                options.SenderName = $"{AppEnvironment.Name} {AppEnvironment.Version}";
            });

            services.AddAutoMapper(typeof(AutoMapperProfile));

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddMvcOptions(opt => { opt.Filters.Add(typeof(NoContentFilter)); })
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver =
                        new Newtonsoft.Json.Serialization.DefaultContractResolver();
                })
                .AddFluentValidation(opt => { opt.RegisterValidatorsFromAssembly(Assembly.GetEntryAssembly()); });

            services.Configure<MvcOptions>(opts =>
            {
                var formatter = opts.OutputFormatters.FirstOrDefault(i => i.GetType() == typeof(JsonOutputFormatter));
                var jsonFormatter = formatter as JsonOutputFormatter;
                var formatterSettings = jsonFormatter == null
                    ? JsonSerializerSettingsProvider.CreateSerializerSettings()
                    : jsonFormatter.PublicSerializerSettings;
                if (formatter != null)
                    opts.OutputFormatters.RemoveType<JsonOutputFormatter>();
                formatterSettings.DateFormatString = "yyyy-MM-ddTHH:mm:ss.fffZ";
                var jsonOutputFormatter = new JsonOutputFormatter(formatterSettings, ArrayPool<char>.Create());
                opts.OutputFormatters.Insert(0, jsonOutputFormatter);
            });

            _appSettings = settingsManager.CurrentValue;

            services.AddSwaggerGen(options =>
            {
                options.DefaultLykkeConfiguration(ApiVersion, ApiTitle);
            });

            // services.Configure<ApiBehaviorOptions>(options =>
            // {
            //     // Wrap failed model state into LykkeApiErrorResponse.
            //     //options.InvalidModelStateResponseFactory = InvalidModelStateResponseFactory.CreateInvalidModelResponse;
            // });

            services.AddLykkeLogging(
                settingsManager.ConnectionString(s => s.AdminApi.Db.LogsConnString),
                "AdminApiServiceLogs",
                _appSettings.SlackNotifications.AzureQueue.ConnectionString,
                _appSettings.SlackNotifications.AzureQueue.QueueName);

            var builder = new ContainerBuilder();

            builder.Populate(services);

            builder.RegisterModule(new AutofacModule(settingsManager.CurrentValue));

            ApplicationContainer = builder.Build();
            
            return new AutofacServiceProvider(ApplicationContainer);
        }

        [UsedImplicitly]
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifetime,
            IMapper mapper)
        {
            try
            {
                app.UseCors(builder => builder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .SetIsOriginAllowed(host => true)
                    .AllowCredentials()
                );

                app.Use(async (context, next) =>
                {
                    if (context.Request.Method == "OPTIONS")
                    {
                        context.Response.StatusCode = 200;
                        await context.Response.WriteAsync("");
                    }
                    else
                    {
                        await next.Invoke();
                    }
                });

                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }

                app.UseLykkeMiddleware(ex => new {message = "Technical problem"});
                app.UseMiddleware<LykkeApiErrorMiddleware>();
                app.UseMiddleware<ClientServiceApiExceptionMiddleware>();

                app.Use(next => context =>
                {
                    context.Request.EnableRewind();

                    return next(context);
                });

                app.UseMvc(routes =>
                {
                    routes.MapRoute(
                        name: "default-to-swagger",
                        template: "{controller=Swagger}");
                });

                app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                });

                app.UseDefaultFiles();

                app.UseSwagger();
                app.UseSwaggerUI(o =>
                {
                    o.RoutePrefix = "swagger/ui";
                    o.SwaggerEndpoint($"/swagger/{ApiVersion}/swagger.json", ApiVersion);
                });

                appLifetime.ApplicationStarted.Register(() => StartApplication().GetAwaiter().GetResult());
                appLifetime.ApplicationStopped.Register(CleanUp);

                mapper.ConfigurationProvider.AssertConfigurationIsValid();
            }
            catch (Exception ex)
            {
                try
                {
                    var log = app.ApplicationServices.GetService<ILogFactory>().CreateLog(typeof(Startup).FullName);
                    log?.Critical(ex);
                }
                catch (Exception e)
                {
                    Console.WriteLine(ex);
                    Console.WriteLine(e);
                }

                throw;
            }
        }

        private async Task StartApplication()
        {
            try
            {
                var healthNotifier = ApplicationContainer.Resolve<IHealthNotifier>();
                healthNotifier.Notify("Started");

                await Configuration.RegisterInMonitoringServiceAsync(
                    _appSettings.MonitoringServiceClient.MonitoringServiceUrl, healthNotifier);
            }
            catch (Exception ex)
            {
                try
                {
                    var log = ApplicationContainer.Resolve<ILogFactory>().CreateLog(typeof(Startup).FullName);
                    log?.Critical(ex);
                }
                catch (Exception e)
                {
                    Console.WriteLine(ex);
                    Console.WriteLine(e);
                }

                throw;
            }
        }

        private void CleanUp()
        {
            try
            {
                // NOTE: Service can't receive and process requests here, so you can destroy all resources
                var healthNotifier = ApplicationContainer.Resolve<IHealthNotifier>();
                healthNotifier.Notify("Terminating");

                ApplicationContainer.Dispose();
            }
            catch (Exception ex)
            {
                try
                {
                    var log = ApplicationContainer.Resolve<ILogFactory>().CreateLog(typeof(Startup).FullName);
                    log?.Critical(ex);
                }
                catch (Exception e)
                {
                    Console.WriteLine(ex);
                    Console.WriteLine(e);
                }

                throw;
            }
        }
    }
}