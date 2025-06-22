using System.Diagnostics;
using Exercicios.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Exercicios.App;

public class Application : IApplication, IHostedService
{
    #region Construtor

    private readonly IServiceProvider ServiceProvider;

    public Relatorio Relatorio { get; set; } = new Relatorio();

    public Application(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;


        var tokenRelatorio = Resolve<ICancellationManager>().RegisterToken("Serviço de Relatorio");
        Relatorio.StartAsync(tokenRelatorio);

        Relatorio.StopAsync(tokenRelatorio);
    }

    private T Resolve<T>() where T : class
    {
        //return (ServiceProvider.GetRequiredService<T>() ?? Activator.CreateInstance<T>());
        return (ServiceProvider.GetRequiredService<T>());
    }

    #endregion

    public void Run()
    {
        LogarDebug(this.GetType(), string.Format("RUN"));

        //var tokenEmail = Resolve<ICancellationManager>().RegisterToken("Serviço de email");

        //var token = Resolve<ICancellationManager>().RegisterToken("Serviço de Testes");

        var tokenEmailTask = Resolve<ICancellationManager>().RegisterToken("Serviço de email Task");





        Task.WhenAll(
            // EmailAniversarioService.EnviarEmail(tokenEmail),
            EmailAniversarioService.EnviarEmailTask(tokenEmailTask)
        //CancellationTest.TesteTaskAsync(token),

        ).ContinueWith(t =>
        {
            if (t.IsFaulted)
            {
                LogarDebug(this.GetType(), $"Erro ao executar tarefas: {t.Exception?.Message}");
            }
            else
            {
                LogarDebug(this.GetType(), "Todas as tarefas concluídas com sucesso.");
            }
        });

        // TODO! Criar mais servicos IHosted de exemplo

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
        LogarDebug(this.GetType(), " StartAsync");

        Run();

        while (!cancellationToken.IsCancellationRequested)
        {
            Console.WriteLine("Pressione Enter para cancelar todos os serviços...");
            var key = Console.ReadLine();

            // TODO! Criar menu para criar serviços

            if (key == "cancelar all")
            {
                Resolve<ICancellationManager>().CancelarTodos();
            }
            else if (key == "all")
            {
                Resolve<ICancellationManager>().ObterTodosOsServicos().ToList().ForEach(s =>
                {
                    LogarDebug(this.GetType(), $"Serviço: {s.Nome} iniciado em {s.Start}", Log.Console);
                });
            }
            else if (key == "cancelar email")
            {
                Resolve<ICancellationManager>().CancelarServico("Serviço de email");
            }
            else if (key == "cancelar servico")
            {
                var service = Console.ReadLine();
                Resolve<ICancellationManager>().CancelarServico(service);
            }
            else if (key == "ativos")
            {
                Resolve<ICancellationManager>().ObterServicosAtivos().ToList().ForEach(s =>
                {
                    LogarDebug(this.GetType(), $"Serviço: {s.Nome} iniciado em {s.Start}", Log.Console);
                });
            }
            else if (key == "host")
            {
                Resolve<ICancellationManager>().CancelarServico("Serviço de Relatorio");


            }
        }

        LogarDebug(this.GetType(), $" em modo assíncrono. {DateTime.Now}");

        LogarDebug(this.GetType(), "Finalizado");

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine(string.Format("stopAsync {0}", typeof(Program)));

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Console.WriteLine(string.Format("stopAsync {0}", typeof(Program)));
        throw new NotImplementedException();
    }

    public static void LogarDebug(Type type, string mensagem, Log log = Log.Debug)
    {
        var msg = string.Format("{0} ---------- {1}", type.Name, mensagem);

        switch (log)
        {
            case Log.All:
                {
                    Console.WriteLine(msg);
                    Debug.WriteLine(msg);
                    break;
                }
            case Log.Debug:
                {
                    Debug.WriteLine(msg);
                    break;
                }
            case Log.Console:
                Console.WriteLine(msg);
                break;

            default:
                {
                    Debug.WriteLine(msg);
                    break;

                }
        }
    }

    public enum Log
    {
        All,
        Debug,
        Console,
    }
}
