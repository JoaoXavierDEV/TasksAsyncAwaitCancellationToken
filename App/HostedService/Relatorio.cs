using Microsoft.Extensions.Hosting;
using static Exercicios.App.Application;

namespace Exercicios.App
{
    public class Relatorio : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            for (var i = 0; i < 1000; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    LogarDebug(typeof(Relatorio), "Operação cancelada pelo usuário.");

                }



                await Task.Delay(1100, cancellationToken); // Simula alguma operação demorada
                LogarDebug(typeof(Relatorio), $"Iteração {i + 1}");
            }

            LogarDebug(typeof(Relatorio), "Operação concluída com êxito.");

            await Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Implementar lógica de finalização do serviço
            Console.WriteLine("Serviço de Relatório parado. aaaaaaaa");
            return Task.CompletedTask;
        }
    }

}
