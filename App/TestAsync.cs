using static Exercicios.App.Application;

namespace Exercicios.App;

public static class CancellationTest
{
    public async static Task TesteTask(CancellationToken cancellationTokenSource)
    {

        var task = Task.Run(() =>
        {
            for (var i = 0; i < 1000; i++)
            {
                if (cancellationTokenSource.IsCancellationRequested)
                {
                    Console.WriteLine("Operação cancelada.");
                    LogarDebug(typeof(CancellationTest), "Operação cancelada pelo usuário.");
                    return;
                }

                // Simula alguma operação demorada
                Thread.Sleep(1100);
                LogarDebug(typeof(CancellationTest), $"Iteração {i + 1}");
            }

            LogarDebug(typeof(CancellationTest), "Operação concluída com êxito.");

        }, cancellationToken: cancellationTokenSource);

        await task;
    }

    public async static Task TesteTaskAsync(CancellationToken cancellationTokenSource)
    {
        // lançou exceção
        for (var i = 0; i < 1000; i++)
        {
            try
            {
                if (cancellationTokenSource.IsCancellationRequested)
                {
                    Console.WriteLine("Operação cancelada.");
                    LogarDebug(typeof(CancellationTest), "Operação cancelada pelo usuário.");
                    return;
                }
                // Simula alguma operação demorada
                await Task.Delay(1100, cancellationTokenSource);
                LogarDebug(typeof(CancellationTest), $"Iteração {i + 1}");
            }
            catch (TaskCanceledException ex)
            {
                // Cancelamento solicitado, apenas sair do loop
                LogarDebug(typeof(CancellationTest), "Serviço cancelado pelo usuário. !! " + ex.Message);
                break;
            }
            catch (Exception ex)
            {

                LogarDebug(typeof(CancellationTest), $"{ex.Message}");
                //throw;
            }
        }
        LogarDebug(typeof(CancellationTest), "Operação concluída com êxito.");

    }

    public static void TesteVoid(CancellationToken cancellationTokenSource)
    {
        // travou o console, mas não lançou exceção
        for (var i = 0; i < 1000; i++)
        {
            if (cancellationTokenSource.IsCancellationRequested)
            {
                Console.WriteLine("Operação cancelada.");
                LogarDebug(typeof(CancellationTest), "Operação cancelada pelo usuário.");
                return;
            }
            // Simula alguma operação demorada
            Thread.Sleep(1100);
            LogarDebug(typeof(CancellationTest), $"Iteração {i + 1}");
        }
        LogarDebug(typeof(CancellationTest), "Operação concluída com êxito.");
    }

    public static void TesteTaskVoid(CancellationToken cancellationTokenSource)
    {
        // nao lançou exceção, mas não é o ideal usar Task.Run em métodos void
        var task = Task.Run(() =>
        {
            for (var i = 0; i < 1000; i++)
            {
                if (cancellationTokenSource.IsCancellationRequested)
                {
                    Console.WriteLine("Operação cancelada.");
                    LogarDebug(typeof(CancellationTest), "Operação cancelada pelo usuário.");
                    return;
                }

                // Simula alguma operação demorada
                Thread.Sleep(1100);
                LogarDebug(typeof(CancellationTest), $"Iteração {i + 1}");
            }

            LogarDebug(typeof(CancellationTest), "Operação concluída com êxito.");

        }, cancellationToken: cancellationTokenSource);


    }
}