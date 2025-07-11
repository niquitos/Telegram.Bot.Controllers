
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
        var token = "5163144004:AAH2qWlP40PEt9sGT3esuaXnrE50-pTZlVQ";//your telegram bot token here
        services.AddTelegramBotControllers(token, (cfg) =>
        {
            cfg.CancelCommand = "стоп";
            cfg.CancelMessage = "Получена команда 'стоп'. Беседа закрыта.";
            cfg.SendMenuText = "Команда не найдена. Доступные команды - в меню";
        });
    })
    .Build();

await host.RunAsync();