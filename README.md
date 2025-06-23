# TasksAsyncAwaitCancellationToken

Este projeto demonstra testes e exemplos de serviços assíncronos em .NET utilizando `CancellationToken` para controle e cancelamento de tarefas.

## Objetivo

Explorar padrões de implementação de tarefas assíncronas, simulação de operações demoradas e cancelamento seguro de execuções, incluindo cenários de uso com `Task`, `Task.Run`, `Task.Delay` e integração com serviços agendados.

## Exemplos de Testes

### 1. Cancelamento de Task com Thread.Sleep

Executa uma tarefa em background, verificando periodicamente se o cancelamento foi solicitado:

### 2. Cancelamento de Task com Task.Delay (Assíncrono)

Utiliza `Task.Delay` para simular operações demoradas e permite cancelamento imediato:

### 3. Serviço de Envio de E-mails Agendado

Serviço que agenda o envio de e-mails diariamente às 9h da manhã, respeitando o cancelamento:

### 4. Execução de Serviço em Background com IHostedService

Exemplo de implementação de serviço agendado usando `BackgroundService` para executar tarefas diariamente:

## Observações

- O cancelamento de tarefas lança `TaskCanceledException`, comportamento esperado e tratado nos exemplos.
- O uso de `CancellationToken` é fundamental para garantir que tarefas longas possam ser interrompidas de forma segura e controlada.
- Os exemplos demonstram tanto padrões síncronos quanto assíncronos, além de integração com serviços hospedados.

## Como Executar

1. Configure o projeto para rodar em .NET 9.
2. Execute os métodos de teste diretamente ou registre o serviço no `Program.cs`:
3. Para testar o cancelamento, chame `cancellationTokenSource.Cancel()` durante a execução.

---

