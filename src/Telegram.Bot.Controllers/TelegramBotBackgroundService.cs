using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Polling;

namespace Telegram.Bot.Controllers;

public class BotBackgroundService : IHostedService
{
    private readonly ITelegramBotClient _botClient;
    private readonly IUpdateHandler _handler;
    private readonly ILogger<BotBackgroundService> _logger;
    private CancellationTokenSource _cts = new();

    public BotBackgroundService(ITelegramBotClient botClient, IUpdateHandler handler, ILogger<BotBackgroundService> logger)
    {
        _botClient = botClient;
        _handler = handler;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting bot service...");

        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = []
        };

        _botClient.StartReceiving(
            updateHandler: _handler,
            receiverOptions: receiverOptions,
            cancellationToken: _cts.Token);
        
        _logger.LogInformation("Telegram bot started.");

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping bot service...");
        _cts.Cancel();
        return Task.CompletedTask;
    }
}
