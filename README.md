# Telegram.Bot.Controllers

A lightweight and easy-to-use C# library for building conversational Telegram bots using a controller-based architecture.

## What It Does

This library helps you build **stateful**, **multi-step** conversations with your Telegram bot in a clean, organized way. It automatically:

- Tracks conversation steps
- Builds dynamic command menus at each step
- Handles cancellation 
- Supports override commands that reset the conversation
- Recovers from invalid input gracefully

All you need to do is define your conversation flow using simple controllers and attributes.

## Getting Started

### Prerequisites

- [.NET 8 SDK or later](https://dotnet.microsoft.com/download )
- A Telegram Bot Token (from [@BotFather](https://t.me/BotFather ))

### Installation

Install via NuGet:

```bash
dotnet add package Telegram.Bot.Controllers
```

## Setup

### Default 

``` csharp
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var token = ""; // Replace with your Telegram bot token
        services.AddTelegramBotControllers(token);
    })
    .Build();

await host.RunAsync();
```

### Customized
``` csharp
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var token = "";//your telegram bot token here
        services.AddTelegramBotControllers(token, (cfg) =>
        {
            cfg.SetCancellation("отмена", "Получена команда 'отмена'.");
            cfg.SetCommandNotFoundText("Команда не найдена. Доступные команды - в меню");
            cfg.SetConversationClosedText("Беседа закрыта");
        });
    })
    .Build();

await host.RunAsync();
```

- **SetCancellation**: set a custom command for cancellation and a message that will be dispayed on cancellation.
- **SetCommandNotFoundText**: set a message the will be displayed when the command was not found. If not set nothing is diplayed
- **SetConversationClosedText**: set a message that will be displayed when the conversation reaches the last step. If not set nothing is displayed

## Writing Controllers
Create classes that inherit from TelegramBotControllerBase, and use the `[Command]` attribute to define your conversation steps. Path segments correspond the conversation steps.

### Example: Commands on Method Level
You can define the full command path directly on a method.

```csharp
public class StartController : TelegramBotControllerBase
{
    [Command("start")] //matches start (top-level command)
    public async Task Start(CancellationToken ct)
    {
        await MessageService.SendMessageAsync(Update.Message.Chat.Id, "Hello! You said 'start'", ct);
    }

    [Command("about")] //matches about (top-level command)
    public async Task Start(CancellationToken ct)
    {
        await MessageService.SendMessageAsync(Update.Message.Chat.Id, "Hello! You said 'about'", ct);
    }

    [Command("start/nested")] //matches start/nested
    public async Task Start(CancellationToken ct)
    {
        await MessageService.SendMessageAsync(Update.Message.Chat.Id, "Hello! You said 'start' then 'nested'", ct);
    }
}
```

### Example: Base paths
You can extract the base path into the classlevel attribute.
```csharp
[Command("start/menu")] //sets the base path
public class StartMenuController : TelegramBotControllerBase
{
    [Command] //matches start/menu
    public async Task Start(CancellationToken ct)
    {
        await MessageService.SendMessageAsync(Update.Message.Chat.Id, "Hello! You said 'start' then 'menu'", ct);
    }

    [Command("nested")] //matches start/menu/nested
    public async Task Start(CancellationToken ct)
    {
        await MessageService.SendMessageAsync(Update.Message.Chat.Id, "Hello! You said 'start' then 'menu' then 'nested'", ct);
    }
}
```

### Example: Override Commands
You can use Override to execute a command regardless of the current step. The previous conversation will be canceled, and the overridden command will take effect.
```csharp
[Command("start/menu")] //sets the base path
public class StartMenuController : TelegramBotControllerBase
{
    [Command] //matches start/menu
    public async Task Start(CancellationToken ct)
    {
        await MessageService.SendMessageAsync(Update.Message.Chat.Id, "Hello! You said 'start' then 'menu'", ct);
    }

    [Command("help", Override = true)] //matches help
    public async Task Start(CancellationToken ct)
    {
        await MessageService.SendMessageAsync(Update.Message.Chat.Id, "Hello! You said 'help'", ct);
    }
}
```

### Base Class Features
Your controllers inherit useful properties:

`MessageService` – For sending messages

`Update` – The current update object

## Benefits
- Easy to understand and maintain
- Keeps your bot logic clean and modular
- Built-in conversation management
- Works out of the box with minimal setup
- Fully supports dependency injection

## Need Help or Want to Contribute?
Feel free to open issues or PRs on GitHub.

## License
MIT License – see the [LICENSE](https://github.com/niquitos/Telegram.Bot.Controllers/blob/main/LICENSE) file.

## Project Links
GitHub: https://github.com/niquitos/Telegram.Bot.Controllers