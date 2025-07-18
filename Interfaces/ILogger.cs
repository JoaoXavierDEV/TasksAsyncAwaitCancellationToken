using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Exercicios.App.NovoLogger;

public class FileLogger : ILogger
{
    private readonly string _categoryName;
    private readonly string _filePath;

    public FileLogger(string categoryName, string filePath)
    {
        _categoryName = categoryName;
        _filePath = filePath;
    }

    public IDisposable BeginScope<TState>(TState state) => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception exception,
        Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        var message = formatter(state, exception);

        var log = $"{timestamp} [{logLevel}] {_categoryName}: {message}";

        File.AppendAllText(_filePath, log + Environment.NewLine);
        Debug.WriteLine(log);
    }
}

public class FileLoggerProvider : ILoggerProvider
{
    private readonly string _filePath;

    public FileLoggerProvider(string filePath)
    {
        _filePath = filePath;
    }

    public ILogger CreateLogger(string categoryName)
    {
        // Ignora logs da Microsoft
        if (categoryName.StartsWith("Microsoft") || categoryName.StartsWith("System"))
            return NullLogger.Instance; // logger que ignora tudo

        var lastName = categoryName.Split('.').Last();

        var fileLog = $"{categoryName}_log.txt";

        var newFilePath = string.Format("{0}{1}", _filePath, fileLog);

        if (File.Exists(newFilePath)) File.Delete(newFilePath);

        return new FileLogger(categoryName, newFilePath);
    }

    public void Dispose() { }
}

//class Program
//{
//    static void Main(string[] args)
//    {
//        using var loggerFactory = LoggerFactory.Create(builder =>
//        {
//            builder.ClearProviders();
//            builder.AddProvider(new FileLoggerProvider("C:\\AppHostedServices\\"));
//            builder.AddDebug(); // se quiser usar Output padrão também
//        });

//        var logger = loggerFactory.CreateLogger<EmailHosted2>();

//        logger.LogInformation("EmailHosted iniciado");
//    }
//}

//public class EmailHosted2 { }