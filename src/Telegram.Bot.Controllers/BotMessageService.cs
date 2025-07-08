using System;

namespace Telegram.Bot.Controllers;

public class BotMessageService
{
    private readonly ITelegramBotClient _client;

    public BotMessageService(ITelegramBotClient client)
    {
        _client = client;
    }
    public async Task SendMessageSync(long chatId, string message, CancellationToken ct)
    {
        await _client.SendMessage(chatId, message, cancellationToken: ct);
    }
}
