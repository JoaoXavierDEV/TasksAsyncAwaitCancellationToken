# TasksAsyncAwaitCancellationToken

Este projeto demonstra testes e exemplos de servi�os ass�ncronos em .NET utilizando `CancellationToken` para controle e cancelamento de tarefas.

## Objetivo

Explorar padr�es de implementa��o de tarefas ass�ncronas, simula��o de opera��es demoradas e cancelamento seguro de execu��es, incluindo cen�rios de uso com `Task`, `Task.Run`, `Task.Delay` e integra��o com servi�os agendados.

## Exemplos de Testes

### 1. Cancelamento de Task com Thread.Sleep

Executa uma tarefa em background, verificando periodicamente se o cancelamento foi solicitado:

### 2. Cancelamento de Task com Task.Delay (Ass�ncrono)

Utiliza `Task.Delay` para simular opera��es demoradas e permite cancelamento imediato:

### 3. Servi�o de Envio de E-mails Agendado

Servi�o que agenda o envio de e-mails diariamente �s 9h da manh�, respeitando o cancelamento:

### 4. Execu��o de Servi�o em Background com IHostedService

Exemplo de implementa��o de servi�o agendado usando `BackgroundService` para executar tarefas diariamente:

## Observa��es

- O cancelamento de tarefas lan�a `TaskCanceledException`, comportamento esperado e tratado nos exemplos.
- O uso de `CancellationToken` � fundamental para garantir que tarefas longas possam ser interrompidas de forma segura e controlada.
- Os exemplos demonstram tanto padr�es s�ncronos quanto ass�ncronos, al�m de integra��o com servi�os hospedados.

## Como Executar

1. Configure o projeto para rodar em .NET 9.
2. Execute os m�todos de teste diretamente ou registre o servi�o no `Program.cs`:
3. Para testar o cancelamento, chame `cancellationTokenSource.Cancel()` durante a execu��o.

---

