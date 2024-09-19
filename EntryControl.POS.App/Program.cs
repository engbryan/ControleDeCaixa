using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.SimpleNotificationService;
using EntryControl.Contracts.Modules.Authentication.Models;
using EntryControl.Contracts.Modules.Authentication.Services;
using EntryControl.Contracts.Services;
using EntryControl.Contracts.Services.Impl;
using EntryControl.POS.Core.Interfaces.Services;
using EntryControl.POS.Core.Models;
using EntryControl.POS.Data.Contexts;
using EntryControl.POS.Data.Repositories;
using EntryControl.POS.Services.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace EntryControl.POS.App
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
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IHealthCheckService, HealthCheckAWSService>();
            services.AddSingleton<ISynchronizeService, SynchronizeService>();
            services.AddSingleton<ISendService, SendAWSService>();
            services.AddSingleton<ISnsClientService, SnsClientService>();
            services.AddScoped<IEntryService, EntryService>();
            services.AddScoped<IEntryRepository, EntryRepository>();
            services.AddScoped<ITimerService, TimerService>();

        }
    }
}