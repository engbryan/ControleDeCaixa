using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.SimpleNotificationService;
using EntryControl.Client;
using EntryControl.Client.Flows;
using EntryControl.Core.Entities;
using EntryControl.Core.Services;
using EntryControl.Core.Services.Impl;
using EntryControl.Entities;
using EntryControl.Forms;
using EntryControl.POS.Data.Contexts;
using EntryControl.Providers;
using EntryControl.Providers.Impl;
using EntryControl.Repositories;
using EntryControl.Repositories.Impl;
using EntryControl.Services;
using EntryControl.Services.Impl;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Refit;
using System;
using System.IO;

namespace EntryControl
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            var serviceCollection = new ServiceCollection();
            var configuration = ConfigureAppSettings();

            ConfigureServices(serviceCollection, configuration);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            using (var serviceScope = serviceProvider.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                context.Database.Migrate();
            }

            var applicationFlow = serviceProvider.GetRequiredService<IApplicationFlow>();

            applicationFlow.Start();
        }

        private static IConfiguration ConfigureAppSettings()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            return builder.Build();
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                //options.UseSqlServer(configuration.GetConnectionString("SQLServerConnection")),
                options.UseSqlite(configuration.GetConnectionString("SqliteConnection")),
                ServiceLifetime.Transient, ServiceLifetime.Transient);

            services.Configure<ExternalServiceOptions>(configuration.GetSection("ExternalServiceOptions"));

            services.AddRefitClient<IEntryApi>()
                    .ConfigureHttpClient(c => c.BaseAddress = new Uri(configuration["ExternalServiceOptions:Url"]));

            services.AddLogging(configure =>
            {
                configure.AddConsole();
                configure.AddDebug();
            });

            services.AddSingleton(configuration.GetSection("CredentialsOptions").Get<CredentialsOptions>());

            services.AddSingleton(RegionEndpoint.USEast1);
            services.AddSingleton<IAWSCredentialsService, EncryptedAWSCredentialsService>();
            services.AddSingleton(sp => sp.GetRequiredService<IAWSCredentialsService>().AWSCredentials);
            services.AddSingleton<IAmazonCognitoIdentityProvider, AmazonCognitoIdentityProviderClient>();
            services.AddSingleton<IAmazonSimpleNotificationService, AmazonSimpleNotificationServiceClient>();

            services.AddSingleton(configuration);
            services.AddSingleton<LoginForm>();
            services.AddSingleton<MainForm>();
            services.AddSingleton<IApplicationFlow, DefaultApplicationFlow>();
            services.AddSingleton<IAuthService, CognitoAuthService>();
            services.AddSingleton<IUserProvider, UserProvider>();
            services.AddSingleton<IHealthCheckService, HealthCheckAWSService>();
            services.AddSingleton<ISynchronizeService, SynchronizeService>();
            services.AddSingleton<ISendService, SendAWSService>();
            services.AddSingleton<ISnsClientService, SnsClientService>();
            services.AddScoped<IEntryService, EntryService>();
            services.AddScoped<IEntryRepository, EntryRepository>();
            services.AddScoped<ITimerProvider, TimerProvider>();

        }
    }
}