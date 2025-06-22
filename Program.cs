using Exercicios.App;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using static Exercicios.DependencyInjection;

namespace Exercicios
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args);

            builder.ConfigureServices(ResolveDependencies).UseConsoleLifetime();

            var app = builder.Build();

            await app.RunAsync();
        }
    }

    public static class DependencyInjection
    {
        public static Action<HostBuilderContext, IServiceCollection> ResolveDependencies = (builder, services) =>
        {
            //services.AddScoped<IGerenciarHoversController, GerenciarHoversController>();
            //services.AddScoped<ILogger, Logger>();
            services.AddScoped<ICancellationManager, CancellationManager>();
            ////services.AddSingleton<IApplication, Application>();
            services.AddHostedService<Application>();
            services.AddHostedService<Relatorio>();
            ////services.AddHostedService<ExecutarInstrucoesAsync>();
            //services.Configure<HostOptions>(option =>
            //{
            //    //option.ShutdownTimeout = System.TimeSpan.FromSeconds(5);
            //    // option.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.StopHost;
            //});
        };
    }
}
