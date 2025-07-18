using System.Diagnostics;
using Exercicios.App;
using Exercicios.App.Command;
using Exercicios.App.HostedService;
using Exercicios.App.NovoLogger;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace Exercicios
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Debug.WriteLine("Iniciando aplicação...");

            var builder = Host.CreateDefaultBuilder(args);

            builder.ConfigureServices(ResolveDependencies).UseConsoleLifetime();

            var app = builder.Build();

            await app.RunAsync();
        }

        private readonly static Action<HostBuilderContext, IServiceCollection> ResolveDependencies = static (builder, services) =>
        {
            services.AddScoped<ICancellationManager, CancellationManager>();
            ////services.AddSingleton<IApplication, Application>();
            services.AddHostedService<EmailHosted>();
            services.AddHostedService<Relatorio>();
            services.AddHostedService<Application>();

            services.AddLogging(config =>
            {
                config.ClearProviders();
                config.AddProvider(new FileLoggerProvider("C:\\AppHostedServices\\"));
                //config.AddConsole(opt =>
                //{
                //    //opt.IncludeScopes = false;

                //});
                config.AddDebug();
                config.SetMinimumLevel(LogLevel.Debug);


                // Filtro: só permite exibir no Console exatamente LogInformation
                config.AddFilter<ConsoleLoggerProvider>(category: null, level => level == LogLevel.Information);

            });
            services.Configure<HostOptions>(option =>
            {
                option.ShutdownTimeout = System.TimeSpan.FromSeconds(2);
                option.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.StopHost;
                option.ServicesStopConcurrently = true;
                option.ServicesStartConcurrently = true;
            });

            // IMenuCommandBase
            services.AddSingleton<IMenuCommandBase, CancelarAllCommand>();
            services.AddSingleton<IMenuCommandBase, AllCommand>();
            services.AddSingleton<IMenuCommandBase, CancelarServicoCommand>();
            services.AddSingleton<IMenuCommandBase, AtivosCommand>();
            services.AddSingleton<IMenuCommandBase, ListarComandosCommand>();


        };
    }

}
