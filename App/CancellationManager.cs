using System.Diagnostics;

namespace Exercicios.App;

public interface ICancellationManager
{
    CancellationToken RegisterToken(string nome);
    CancellationToken ObterToken(string nome);
    void CancelarTodos();
    void CancelarToken(string nome);
    IEnumerable<Token> ObterTokensAtivos();
    IEnumerable<Token> ObterTodosOsTokens();
}

public class CancellationManager : ICancellationManager
{
    private readonly IServiceProvider ServiceProvider;
    public List<Token> Tokens { get; set; } = new List<Token>();

    public CancellationManager(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    public CancellationManager()
    {
    }

    public CancellationToken RegisterToken(string nome)
    {
        var cts = new CancellationTokenSource();

        cts.Token.Register(() =>
        {
            Debug.WriteLine($"Token {nome} cancelado pelo usuário.");
        });

        var service = new Token(nome, cts);

        Tokens.Add(service);

        return cts.Token;
    }

    public void CancelarTodos()
    {
        foreach (var x in Tokens)
        {
            x.CancellationToken.Cancel();
        }
    }

    public IEnumerable<Token> ObterTokensAtivos()
    {
        // Retorna apenas os que ainda não foram cancelados
        foreach (var cts in Tokens)
        {
            if (!cts.CancellationToken.IsCancellationRequested)
                yield return cts;
        }
    }

    public IEnumerable<Token> ObterTodosOsTokens()
    {
        //var Servicos = ServiceProvider;

        return Tokens;
    }
    // se houver um token com o mesmo nome eparametros, nao executar
    // se houver 
    public void CancelarToken(string nome)
    {
        var Token = ObterTodosOsTokens().FirstOrDefault(s => s.Nome == nome);
        if (Token != null)
        {
            Token.CancellationToken.Cancel();
            RemoverToken(Token);
        }

    }

    private void RemoverToken(Token Token)
    {
        if (Token is null) return;
        if (!Tokens.Contains(Token)) return;
        if (!Token.CancellationToken.IsCancellationRequested) return;

        Tokens.Remove(Token);
    }

    public CancellationToken ObterToken(string nome)
    {
        return Tokens.FirstOrDefault(s => s.Nome == nome)?.CancellationToken.Token ?? throw new ArgumentException($"Serviço {nome} não encontrado.", nameof(nome));
    }
}


public class Token(string nome, CancellationTokenSource cancellationToken)
{
    public string Nome { get; set; } = nome;
    public DateTime Start { get; set; } = DateTime.Now;
    public CancellationTokenSource CancellationToken { get; set; } = cancellationToken;
    public override string ToString() => $"{Nome} - {Start} - {CancellationToken.IsCancellationRequested}";
}
