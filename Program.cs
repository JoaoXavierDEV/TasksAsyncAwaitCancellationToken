var cancellationTokenSource = new CancellationTokenSource();

cancellationTokenSource.Token.Register(() =>
{
    // action é executada no momento do .Cancel()
    Console.WriteLine("Operação cancelada pelo usuário");
});


Console.WriteLine("Para cancelar pressione Enter/Return...");

var task = Task.Run(() =>
{
    for (var i = 0; i < 10; i++)
    {
        if (cancellationTokenSource.Token.IsCancellationRequested)
        {
            Console.WriteLine("Operação cancelada.");
            return;
        }

        // Simula alguma operação demorada
        Thread.Sleep(1100);
        Console.WriteLine($"Iteração {i + 1}");
    }

    Console.WriteLine("Operação concluída com êxito.");
});

Console.ReadLine();

Console.WriteLine(cancellationTokenSource.Token.IsCancellationRequested);

cancellationTokenSource = null;

cancellationTokenSource = new CancellationTokenSource();

cancellationTokenSource.Cancel();


Console.WriteLine(cancellationTokenSource.Token.IsCancellationRequested);

await task;

Console.ReadLine();