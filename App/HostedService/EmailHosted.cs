using Microsoft.Extensions.Hosting;
using static Exercicios.App.Application;

namespace Exercicios.App.HostedService
{
    public class EmailHosted : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Vincula o token de cancelamento atual ao novo token
            var newToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            // Atualiza o token de cancelamento para o novo token vinculado
            cancellationToken = newToken.Token;

            LogarDebug(typeof(EmailHosted), "EmailHosted iniciado");

            await Task.Run(async () =>
            {
                try
                {
                    // Inicia o serviço de envio de e-mails de aniversário
                    //await EmailAniversarioService.EnviarEmail(cancellationToken);

                    for (var i = 0; i < 100; i++)
                    {
                        // Delay de 5 segundos para cada incremento
                        // para simular uma operação longa

                        // os HostedService são executados em fila?
                        if (DateTime.Now.Second % 2 == 0)
                        {
                            throw new TimeoutException("erooorroorororor");
                        }

                        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                        LogarDebug(typeof(Relatorio), $"Simula uma query longa {i + 1}");
                    }
                }
                catch (TaskCanceledException ex)
                {
                    LogarDebug(typeof(EmailHosted), "Serviço de email cancelado pelo usuário. !! " + ex.Message);
                }
                catch (Exception ex)
                {
                    LogarDebug(typeof(EmailHosted), $"Erro no serviço de email: {ex.Message}");
                }
            }, cancellationToken);




            ////await EmailAniversarioService.EnviarEmailTask(cancellationToken);

            //newToken.Cancel();
            //await this.StopAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            /*
            Como testar o StopAsync corretamente
            •	Pare a aplicação(Ctrl + C no terminal, ou feche o processo).
            •	Ou chame explicitamente IHost.StopAsync() no seu código.
            Aí sim, o host irá chamar o método StopAsync do seu serviço.
            */

            LogarDebug(typeof(EmailHosted), "Operação cancelada pelo usuário. StopAsync");

            //Console.WriteLine("Serviço de Relatório parado. StopAsync");

            return Task.CompletedTask;
        }
    }
}
