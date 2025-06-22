using System.Diagnostics;
using Exercicios.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Exercicios.App;

public class Application : IApplication, IHostedService
{
    #region Construtor

    private readonly IServiceProvider ServiceProvider;

    public Application(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
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

        var tokenEmail = Resolve<ICancellationManager>().RegisterToken("Serviço de email");

        EmailAniversarioService.EnviarEmail(tokenEmail);

        // TODO! Criar mais servicos de exemplo
        var token = Resolve<ICancellationManager>().RegisterToken("Serviço de Testes");

        CancellationTest.TesteTaskAsync(token);

        // TODO! Criar mais servicos IHosted de exemplo

    }

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
                    LogarDebug(this.GetType(), $"Serviço: {s.Nome} iniciado em {s.Start}");
                });
            }
            else if (key == "cancelar email")
            {
                Resolve<ICancellationManager>().CancelarServico("Serviço de email");
            }
            else if (key == "ativos")
            {
                Resolve<ICancellationManager>().ObterServicosAtivos().ToList().ForEach(s =>
                {
                    LogarDebug(this.GetType(), $"Serviço: {s.Nome} iniciado em {s.Start}");
                });
            }
        }
        /*
                    var client = new ExecutarInstrucoesAsync();
                    await client.StartAsync(cancellationToken);
        */
        LogarDebug(this.GetType(), $" em modo assíncrono. {DateTime.Now}");

        LogarDebug(this.GetType(), "Finalizado");

        // await client.StopAsync(cancellationToken);

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

    public static void LogarDebug(Type type, string mensagem)
    {
        var msg = string.Format("{0} ---------- {1}", type.Name, mensagem);
        //Console.WriteLine(msg);
        Debug.WriteLine(msg);
    }
}
