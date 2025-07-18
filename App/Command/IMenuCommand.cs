using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Exercicios.App.Command;

public interface IMenuCommandBase
{
    public abstract string Nome { get; }
    public abstract string? Descricao { get; }
    void TryExecute(string input);
    public bool CanExecute(string input);
    public void Execute();
}

public interface IMenuCommand<T> : IMenuCommandBase
{
    ICancellationManager CancellationManager { get; }
    ILogger<T> Logger { get; }
}

public abstract class MenuCommandBase<T> : IMenuCommand<T>
{
    public abstract string Nome { get; }
    public abstract string? Descricao { get; }

    public ICancellationManager CancellationManager { get; }
    public ILogger<T> Logger { get; }

    // TODO! método pode ser renomeado para IsValidCommand
    // pode ter implementação já na abstração
    public virtual bool CanExecute(string input) => string.Equals(input, Nome, StringComparison.InvariantCultureIgnoreCase);
    public abstract void Execute();

    public void TryExecute(string input)
    {
        if (CanExecute(input))
        {
            Execute();
        }
        else
            Logger.LogWarning("Comando não reconhecido: {input}", input);
    }


    protected MenuCommandBase(ICancellationManager cancellationManager, ILogger<T> logger)
    {
        CancellationManager = cancellationManager ?? throw new ArgumentNullException(nameof(cancellationManager));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

}

public class CancelarCommand : MenuCommandBase<CancelarCommand>
{
    public CancelarCommand(ICancellationManager cancellationManager, ILogger<CancelarCommand> logger)
        : base(cancellationManager, logger) { }

    public override string Nome { get; } = "Cancelar";
    public override string? Descricao { get; } = "Cancela um serviço específico. Exemplo: 'cancelar email' cancela o serviço de email.";

    public override bool CanExecute(string input) => input.Trim().StartsWith(Nome, StringComparison.InvariantCultureIgnoreCase);
    public override void Execute()
    {
        string? service = Console.ReadLine();
        ArgumentNullException.ThrowIfNullOrWhiteSpace(service, nameof(service));
        CancellationManager.CancelarToken(service);
    }

    //public override string GetDescription() => "";
}


public class CancelarAllCommand : MenuCommandBase<CancelarAllCommand>
{
    public CancelarAllCommand(ICancellationManager cancellationManager, ILogger<CancelarAllCommand> logger) : base(cancellationManager, logger) { }

    public override string Nome => "Cancelar todos";

    public override string? Descricao => "Cancela todos os tokens";

    public override void Execute() => CancellationManager.CancelarTodos();
}

public class AllCommand : MenuCommandBase<AllCommand>
{
    public AllCommand(ICancellationManager cancellationManager, ILogger<AllCommand> logger) : base(cancellationManager, logger) { }

    public override string Nome => "Obter todos os tokens";

    public override string? Descricao => "Obter todos os tokens";

    public override void Execute()
    {
        foreach (var s in CancellationManager.ObterTodosOsTokens())
            Logger.LogInformation($"Serviço: {s.Nome} iniciado em {s.Start}");
    }
}



public class CancelarServicoCommand : MenuCommandBase<CancelarServicoCommand>
{
    public CancelarServicoCommand(ICancellationManager cancellationManager, ILogger<CancelarServicoCommand> logger) : base(cancellationManager, logger) { }

    public override string Nome => "Cancelar";

    public override string? Descricao => "Cancela o serviço especificado";

    public override bool CanExecute(string input)
    {
        var comando = input.Trim();

        // Divide por espaços
        var partes = comando.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);

        // Separa o nome do comando e o restante
        var nomeComando = partes.Length > 0 ? partes[0] : string.Empty;
        var parametros = partes.Length > 1 ? partes[1] : string.Empty;

        return nomeComando.Equals(Nome, StringComparison.InvariantCultureIgnoreCase) && !string.IsNullOrWhiteSpace(parametros);
    }
    public override void Execute()
    {
        string? service = Console.ReadLine();
        ArgumentNullException.ThrowIfNullOrWhiteSpace(service, nameof(service));
        CancellationManager.CancelarToken(service);
    }
}

public class AtivosCommand : MenuCommandBase<AtivosCommand>
{
    public AtivosCommand(ICancellationManager cancellationManager, ILogger<AtivosCommand> logger) : base(cancellationManager, logger) { }

    public override string Nome => "Listar ativos";

    public override string? Descricao => "Listar CancellationTokens ativos";

    public override void Execute()
    {
        foreach (var s in CancellationManager.ObterTokensAtivos())
            //Logger.LogInformation($"Serviço: {s.Nome} iniciado em {s.Start}");
            Console.WriteLine($"==> Serviço: {s.Nome} iniciado em {s.Start}");
    }
}

public class ListarComandosCommand : MenuCommandBase<ListarComandosCommand>
{
    private readonly IServiceProvider ServiceProvider;

    public ListarComandosCommand(ICancellationManager cancellationManager, ILogger<ListarComandosCommand> logger, IServiceProvider serviceProvider) :
        base(cancellationManager, logger)
    {
        ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public override string Nome => "Listar comandos";

    public override string? Descricao => "Listar comandos disponíveis no terminal";

    public override void Execute()
    {
        IEnumerable<IMenuCommandBase> _commands;

        _commands = ServiceProvider.GetServices<IMenuCommandBase>()
            ?? throw new InvalidOperationException("Nenhum comando registrado.");

        _commands.ToList().ForEach((c) =>
        {
            //Logger.LogInformation($"{c.Nome} - {c.Descricao}");
            Console.WriteLine($" ==> {c.Nome}   -   {c.Descricao}");
        });
    }
}