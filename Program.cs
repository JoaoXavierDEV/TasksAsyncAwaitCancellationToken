using Exercicios.App;
using Exercicios.App.Command;
using Exercicios.App.NovoLogger;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace Exercicios;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = Host.CreateDefaultBuilder(args);

        builder.ConfigureServices(ResolveDependencies).UseConsoleLifetime();

        var app = builder.Build();

        await app.RunAsync();
    }

    private readonly static Action<HostBuilderContext, IServiceCollection> ResolveDependencies = static (builder, services) =>
    {
        services.AddScoped<ICancellationManager, CancellationManager>();
        ////services.AddSingleton<IApplication, Application>();
        //services.AddHostedService<EmailHosted>();
        //services.AddHostedService<Relatorio>();
        services.AddHostedService<Application>();

        services.AddLogging(config =>
        {
            config.ClearProviders(); // se limpar os providers o Information não aparece no console
            config.AddProvider(new FileLoggerProvider("C:\\AppHostedServices\\"));
            config.AddConsole();//.AddDebug(); // Adiciona um prefixo as mensagens "Debug:"
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
        services.AddTransient<IMenuCommandBase, GetTokenCommand>();

        // _logger.LogDebug         // escreve na janela de saida do visual studio
        // _logger.LogInformation   // escreve na janela de saida do visual studio - escreve no console
    };
}
