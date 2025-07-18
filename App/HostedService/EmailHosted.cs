using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Exercicios.App.HostedService
{
    public class EmailHosted : IHostedService
    {
        private readonly ILogger<EmailHosted> _logger;

        public EmailHosted(ILogger<EmailHosted> logger)
        {
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Vincula o token de cancelamento atual ao novo token
            var newToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            // Atualiza o token de cancelamento para o novo token vinculado
            cancellationToken = newToken.Token;

            _logger.LogDebug("EmailHosted iniciado");

            await Task.Run(async () =>
            {
                try
                {
                    // Inicia o serviço de envio de e-mails de aniversário
                    //await EmailAniversarioService.EnviarEmailTask(cancellationToken);

                    await Task.Run(async () =>
                    {
                        _logger.LogDebug("Iniciando o serviço de envio de e-mails de aniversário...");

                        // Calcula o tempo até as 9h da manhã do próximo dia (ou hoje, se ainda não passou)
                        var agora = DateTime.Now;
                        var proximaExecucao = new DateTime(agora.Year, agora.Month, agora.Day, 9, 0, 0);

                        if (agora > proximaExecucao)
                            proximaExecucao = proximaExecucao.AddDays(1);

                        var delay = proximaExecucao - agora;



                        _logger.LogDebug("Próxima execução agendada para: {proximaExecucao} (em {delay.TotalMinutes} minutos)", proximaExecucao, delay.TotalMinutes);
                        try
                        {
                            // Aguarda até a próxima execução
                            await Task.Delay(delay, cancellationToken);

                            // Sua lógica diária aqui
                            _logger.LogDebug("Email de Aniversário enviado para os clientes em: {0}", DateTime.Now.ToString());

                            // Aguarda 24 horas para a próxima execução
                            await Task.Delay(TimeSpan.FromHours(24), cancellationToken);
                        }
                        catch (TaskCanceledException ex)
                        {
                            _logger.LogDebug("Serviço de email cancelado pelo usuário. !! " + ex.Message);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogDebug($"Erro no serviço: {ex.Message}");
                        }


                    }, cancellationToken);
                }
                catch (TaskCanceledException ex)
                {
                    _logger.LogDebug("Serviço de email cancelado pelo usuário. !! " + ex.Message);

                }
                catch (Exception ex)
                {
                    _logger.LogDebug($"Erro no serviço de email: {ex.Message}");
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

            //LogarDebug(typeof(EmailHosted), "Operação cancelada pelo usuário. StopAsync");

            _logger.LogDebug("EmailHosted finalizado");

            return Task.CompletedTask;
        }
    }
}
