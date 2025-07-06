
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Telegram.Bot.Controllers;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .WriteTo.Console()
    .CreateLogger();

var host = Host.CreateDefaultBuilder(args)
    .UseSerilog()
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