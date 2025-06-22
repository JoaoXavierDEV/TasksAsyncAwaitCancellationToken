using static Exercicios.App.Application;

namespace Exercicios.App;

/// <summary>
/// Classe responsável por enviar e-mails de aniversário diariamente às 9h da manhã.
/// </summary>
public static class EmailAniversarioService
{
    /// <summary>
    /// Método responsável por enviar e-mails de aniversário.
    /// </summary>
    /// <param name="cancellationTokenSource"></param>
    /// <returns></returns>
    public async static Task EnviarEmail(CancellationToken cancellationTokenSource)
    {
        while (!cancellationTokenSource.IsCancellationRequested)
        {
            LogarDebug(typeof(EmailAniversarioService), "Iniciando o serviço de envio de e-mails de aniversário...");
            // Calcula o tempo até as 9h da manhã do próximo dia (ou hoje, se ainda não passou)
            var agora = DateTime.Now;
            var proximaExecucao = new DateTime(agora.Year, agora.Month, agora.Day, 9, 0, 0);

            if (agora > proximaExecucao)
                proximaExecucao = proximaExecucao.AddDays(1);

            var delay = proximaExecucao - agora;

            LogarDebug(typeof(EmailAniversarioService), $"Próxima execução agendada para: {proximaExecucao} (em {delay.TotalMinutes} minutos)");
            try
            {
                // Aguarda até a próxima execução
                await Task.Delay(delay, cancellationTokenSource);

                // Sua lógica diária aqui
                LogarDebug(typeof(EmailAniversarioService), $"Email de Aniversário enviado para os clientes em: {DateTime.Now}");

                // Aguarda 24 horas para a próxima execução
                await Task.Delay(TimeSpan.FromHours(24), cancellationTokenSource);
            }
            catch (TaskCanceledException ex)
            {
                // Cancelamento solicitado, apenas sair do loop
                LogarDebug(typeof(EmailAniversarioService), "Serviço cancelado pelo usuário. !! " + ex.Message);
                break;
            }
            catch (Exception ex)
            {
                // Log de erro, se necessário
                LogarDebug(typeof(EmailAniversarioService), $"Erro no serviço: {ex.Message}");
            }
        }
    }


}