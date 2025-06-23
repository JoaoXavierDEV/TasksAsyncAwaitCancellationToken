using System.Diagnostics;

namespace Exercicios.App;

public interface ICancellationManager
{
    CancellationToken RegisterToken(string nomeServico);
    CancellationToken ObterToken(string nomeServico);
    void CancelarTodos();
    void CancelarServico(string nomeServico);
    IEnumerable<Servico> ObterServicosAtivos();
    IEnumerable<Servico> ObterTodosOsServicos();
}

public class CancellationManager : ICancellationManager
{
    public List<Servico> Servicos { get; set; } = new List<Servico>();

    public CancellationToken RegisterToken(string nomeServico)
    {
        var cts = new CancellationTokenSource();

        cts.Token.Register(() =>
        {
            Debug.WriteLine($"Serviço {nomeServico} cancelado pelo usuário.");
        });

        var service = new Servico(nomeServico, DateTime.Now, cts);

        Servicos.Add(service);

        return cts.Token;
    }

    public void CancelarTodos()
    {
        foreach (var x in Servicos)
        {
            x.CancellationToken.Cancel();
        }
    }

    public IEnumerable<Servico> ObterServicosAtivos()
    {
        // Retorna apenas os que ainda não foram cancelados
        foreach (var cts in Servicos)
        {
            if (!cts.CancellationToken.IsCancellationRequested)
                yield return cts;
        }
    }

    public IEnumerable<Servico> ObterTodosOsServicos() => Servicos;

    public void CancelarServico(string nomeServico)
    {
        var servico = ObterTodosOsServicos().FirstOrDefault(s => s.Nome == nomeServico);
        if (servico != null)
        {
            servico.CancellationToken.Cancel();
            RemoverServico(servico);
        }

    }

    private void RemoverServico(Servico servico)
    {
        if (servico == null) return;
        if (!Servicos.Contains(servico)) return;
        if (!servico.CancellationToken.IsCancellationRequested) return;

        Servicos.Remove(servico);
    }

    public CancellationToken ObterToken(string nomeServico)
    {
        return Servicos.FirstOrDefault(s => s.Nome == nomeServico)?.CancellationToken.Token ?? throw new ArgumentException($"Serviço {nomeServico} não encontrado.", nameof(nomeServico));
    }
}


public class Servico
{
    public Servico(string nome, DateTime start, CancellationTokenSource cancellationToken)
    {
        Nome = nome;
        Start = start;
        CancellationToken = cancellationToken;
    }

    public string Nome { get; set; } = string.Empty;
    public DateTime Start { get; set; } = DateTime.Now;
    public CancellationTokenSource CancellationToken { get; set; }
}
