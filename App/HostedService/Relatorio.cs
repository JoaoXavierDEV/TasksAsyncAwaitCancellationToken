using Microsoft.Extensions.Hosting;
using static Exercicios.App.Application;

namespace Exercicios.App
{
    public class Relatorio : IHostedService
    {
        /// <summary>
        /// Serviço asíncrono que simula a geração de um relatório longo.
        /// Caso o usuário cancele a operação, o serviço irá parar de executar.
        /// Caso o tempo de execução ultrapasse 2 minutos, o serviço será cancelado automaticamente.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Vincula o token de cancelamento atual ao novo token
            var newToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            // Atualiza o token de cancelamento para o novo token vinculado
            cancellationToken = newToken.Token;

            LogarDebug(typeof(Relatorio), "Relatório iniciado");

            // Tarefa principal do serviço
            var tarefaPrincipal = ExecutarRelatorioAsync(cancellationToken);

            // Tarefa de timeout (2 minutos)
            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);

            // Aguarda a primeira que terminar
            var completedTask = await Task.WhenAny(tarefaPrincipal, timeoutTask);

            if (completedTask == timeoutTask)
            {
                LogarDebug(typeof(Relatorio), "TimeOutException");
                newToken.Cancel();

                //throw new TimeoutException("Query excedeu o tempo limite por isso foi cancelada automaticamente");
                return;
            }

            // Se chegou aqui, a tarefa principal terminou antes do timeout
            await tarefaPrincipal;
        }

        private async Task ExecutarRelatorioAsync(CancellationToken cancellationToken)
        {
            await Task.Run(async () =>
            {
                try
                {
                    for (var i = 0; i < 10; i++)
                    {
                        // Delay de 5 segundos para cada incremento
                        // para simular uma operação longa

                        // os HostedService são executados em fila?

                        await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
                        LogarDebug(typeof(Relatorio), $"Simula uma query longa {i + 1}");
                    }

                    LogarDebug(typeof(Relatorio), "Operação concluída com êxito.");
                }
                catch (TaskCanceledException ex)
                {
                    LogarDebug(typeof(Relatorio), ex.Message);
                }
            }, cancellationToken);



        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            /*
            Como testar o StopAsync corretamente
            •	Pare a aplicação(Ctrl + C no terminal, ou feche o processo).
            •	Ou chame explicitamente IHost.StopAsync() no seu código.
            Aí sim, o host irá chamar o método StopAsync do seu serviço.
            */

            LogarDebug(typeof(Relatorio), "Operação cancelada pelo usuário. StopAsync");

            //Console.WriteLine("Serviço de Relatório parado. StopAsync");

            return Task.CompletedTask;
        }
    }

}
