using Microsoft.Extensions.Logging;

namespace Exercicios.App;

/// <summary>
/// Classe responsável por enviar e-mails de aniversário diariamente às 9h da manhã.
/// </summary>
public class EmailAniversarioService
{
    private readonly ILogger<EmailAniversarioService> _logger;

    public EmailAniversarioService(ILogger<EmailAniversarioService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Método responsável por enviar e-mails de aniversário diariamente às 9h da manhã.
    /// Forma simples usando while
    /// </summary>
    /// <param name="cancellationTokenSource"></param>
    /// <returns></returns>
    public async Task EnviarEmail(CancellationToken cancellationTokenSource)
    {
        while (!cancellationTokenSource.IsCancellationRequested)
        {
            _logger.LogDebug("Iniciando o serviço de envio de e-mails de aniversário...");
            // Calcula o tempo até as 9h da manhã do próximo dia (ou hoje, se ainda não passou)
            DateTime agora = DateTime.Now;
            DateTime proximaExecucao = new(agora.Year, agora.Month, agora.Day, 9, 0, 0);

            if (agora > proximaExecucao)
            {
                proximaExecucao = proximaExecucao.AddDays(1);
            }

            TimeSpan delay = proximaExecucao - agora;

            _logger.LogDebug("Próxima execução agendada para: {proximaExecucao} (em {delay.TotalMinutes} minutos)", proximaExecucao, delay);

            try
            {
                // Aguarda até a próxima execução
                await Task.Delay(delay, cancellationTokenSource);

                // Sua lógica diária aqui
                _logger.LogDebug("Email de Aniversário enviado para os clientes em: {0}", DateTime.Now.ToString());

                // Aguarda 24 horas para a próxima execução
                await Task.Delay(TimeSpan.FromHours(24), cancellationTokenSource);
            }
            catch (TaskCanceledException ex)
            {
                // Cancelamento solicitado, apenas sair do loop
                _logger.LogDebug($"Serviço cancelado pelo usuário. !! {ex.Message}");
                break;
            }
            catch (Exception ex)
            {
                // Log de erro, se necessário
                _logger.LogDebug($"Erro no serviço: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Método responsável por enviar e-mails de aniversário diariamente às 9h da manhã.
    /// Usando TASK, forma recomenada: await Task.Run(async () => {} , cancellationTokenSource) 
    /// </summary>
    /// <param name="cancellationTokenSource"></param>
    /// <returns></returns>
    public async Task EnviarEmailTask(CancellationToken cancellationTokenSource)
    {
        await Task.Run(async () =>
        {
            _logger.LogDebug($"TASK Iniciando o serviço de envio de e-mails de aniversário...");

            // Calcula o tempo até as 9h da manhã do próximo dia (ou hoje, se ainda não passou)
            DateTime agora = DateTime.Now;
            DateTime proximaExecucao = new(agora.Year, agora.Month, agora.Day, 9, 0, 0);

            if (agora > proximaExecucao)
            {
                proximaExecucao = proximaExecucao.AddDays(1);
            }

            TimeSpan delay = proximaExecucao - agora;

            _logger.LogDebug("Próxima execução agendada para: {proximaExecucao} (em {delay.TotalMinutes} minutos)", proximaExecucao, delay);
            try
            {
                // Aguarda até a próxima execução
                await Task.Delay(delay, cancellationTokenSource);

                // Sua lógica diária aqui
                _logger.LogDebug("TASK - " + $"Email de Aniversário enviado para os clientes em: {DateTime.Now}");

                // Aguarda 24 horas para a próxima execução
                await Task.Delay(TimeSpan.FromHours(24), cancellationTokenSource);
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogDebug("TASK - " + "Serviço cancelado pelo usuário. !! " + ex.Message);
            }
            catch (Exception ex)
            {

                _logger.LogDebug("TASK - " + $"Erro no serviço: {ex.Message}");
            }


        }, cancellationTokenSource);

    }


}