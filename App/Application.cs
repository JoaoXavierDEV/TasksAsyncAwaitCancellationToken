using Exercicios.App.Command;
using Exercicios.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Exercicios.App;

public class Application : IApplication, IHostedService
{
    #region Construtor

    private readonly ILogger<Application> _logger;
    private readonly IServiceProvider ServiceProvider;
    private readonly IEnumerable<IMenuCommandBase> _commands;
    private readonly IEnumerable<ILoggerProvider> _loggingBuilders;

    public Application(
        IServiceProvider serviceProvider,
        ILogger<Application> logger,
        IEnumerable<IMenuCommandBase> commands,
        IEnumerable<ILoggerProvider> loggingBuilders)
    {
        ServiceProvider = serviceProvider;
        _logger = logger;
        _commands = commands;
        _loggingBuilders = loggingBuilders;
    }

    private T Resolve<T>() where T : class
    {
        //return (ServiceProvider.GetRequiredService<T>() ?? Activator.CreateInstance<T>());
        return (ServiceProvider.GetRequiredService<T>());
    }

    #endregion

    public void Run()
    {
        _logger.LogInformation($" - App Run.");

        var tokenEmail = Resolve<ICancellationManager>().RegisterToken("Serviço de email");
        tokenEmail.Instancia = new EmailAniversarioService(Resolve<ILogger<EmailAniversarioService>>()).EnviarEmail(tokenEmail.CancellationToken.Token);

        var tokenEmailtask = Resolve<ICancellationManager>().RegisterToken("Serviço de email task");
        tokenEmailtask.Instancia = new EmailAniversarioService(Resolve<ILogger<EmailAniversarioService>>()).EnviarEmailTask(tokenEmailtask.CancellationToken.Token);


        //var token = Resolve<ICancellationManager>().RegisterToken("Serviço de Relatorio");

        //var tokenEmailTask = Resolve<ICancellationManager>().RegisterToken("Serviço de email Task");


        // TODO adicionar dto de parametros de consulta, info de entrada da viewModel

        Task.WhenAll(

        //new EmailAniversarioService(Resolve<ILogger<EmailAniversarioService>>()).EnviarEmailTask(tokenEmailTask),
        //new CancellationTest(Resolve<ILogger<CancellationTest>>()).TesteTaskAsync(token),

        //token.Instancia = new Relatorio(Resolve<ILogger<Relatorio>>()).StartAsync(token.CancellationToken.Token)



        ).ContinueWith(t =>
        {
            //if (t.IsFaulted)
            //{
            //    _logger.LogError(t.Exception, "Erro ao executar tarefas assíncronas.");
            //}
            //else
            //{
            //    _logger.LogInformation("Todas as tarefas concluídas com sucesso.");
            //}
        });

    }


    /// <summary>
    /// Starts the asynchronous operation for managing services and handles user input for service control.
    /// </summary>
    /// <remarks>This method runs a loop that listens for user input to control services, such as starting,
    /// stopping, or querying their status. The operation continues until the provided <paramref
    /// name="cancellationToken"/> signals cancellation.</remarks>
    /// <param name="cancellationToken">A token that can be used to signal the cancellation of the operation.</param>
    /// <returns>A completed <see cref="Task"/> when the operation finishes or is canceled.</returns>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.BeginScope("Iniciando Application services.");

        Run();

        while (!cancellationToken.IsCancellationRequested)
        {
            Console.WriteLine();
            _commands.First(x => x.Nome == "Listar comandos").Execute();
            Console.WriteLine();
            Console.WriteLine("Digite um comando: ");

            string key = Console.ReadLine() ?? string.Empty;

            var command = _commands.FirstOrDefault(c => c.CanExecute(key));

            if (command != null)
            {
                Console.WriteLine();
                Console.WriteLine("===========================================================================");
                command.Execute();
                Console.WriteLine("===========================================================================");
            }
            else
            {
                Console.WriteLine("Comando não reconhecido.");
            }
        }

        _logger.LogInformation($" - Modo assíncrono. {DateTime.Now}");
        _logger.LogInformation("Finalizado");

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping Application services.");

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _logger.LogInformation("Disposing Application resources.");
    }


}
