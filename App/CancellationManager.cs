using System.Diagnostics;

namespace Exercicios.App;

public interface ICancellationManager
{
    Token RegisterToken(string nome);
    Token ObterToken(string nome);
    void CancelarTodos();
    void CancelarToken(string nome);
    void CancelarToken(Token token);
    IEnumerable<Token> ObterTokensAtivos();
    IEnumerable<Token> ObterTodosOsTokens();
}

public class CancellationManager : ICancellationManager
{
    private readonly IServiceProvider ServiceProvider;
    private readonly List<Token> Tokens = new List<Token>();

    public CancellationManager(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }


    public Token RegisterToken(string nome)
    {
        var cts = new CancellationTokenSource();

        cts.Token.Register(() =>
        {
            Debug.WriteLine($"Token {nome} cancelado pelo usuário.");
        });

        var tk = new Token(nome, cts);

        Tokens.Add(tk);

        return tk;
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
        return Tokens;
    }

    public void CancelarToken(string nome)
    {
        var Token = ObterTodosOsTokens().FirstOrDefault(s => string.Equals(s.Nome, nome, StringComparison.InvariantCultureIgnoreCase));

        if (Token is null)
        {
            Console.WriteLine($"Token {nome} não encontrado.", nameof(nome));
            return;
        }

        CancelarToken(Token);
    }

    public void CancelarToken(Token token)
    {
        if (token is null)
            throw new ArgumentNullException(nameof(token), "Token não pode ser nulo.");

        token.CancellationToken.Cancel();

        //RemoverToken(token);
    }

    private void RemoverToken(Token Token)
    {
        if (Token is null) return;
        if (!Tokens.Contains(Token)) return;
        if (!Token.CancellationToken.IsCancellationRequested) return;

        Tokens.Remove(Token);
    }

    public Token ObterToken(string nome)
    {
        return Tokens.FirstOrDefault(s => s.Nome == nome)
                ?? throw new ArgumentException($"Token {nome} não encontrado.", nameof(nome));
    }
}


public class Token
{
    public string Nome { get; set; }
    public DateTime Start { get; set; } = DateTime.Now;
    public CancellationTokenSource CancellationToken { get; set; }
    public IViewModel ViewModel { get; set; }
    public Task Instancia { get; set; }

    public Token(string nome, CancellationTokenSource cancellationToken)
    {
        Nome = nome;
        CancellationToken = cancellationToken;
    }

    public override string ToString() => $"{Nome} - {Start} - {CancellationToken.IsCancellationRequested}";

}

public interface IViewModel { }