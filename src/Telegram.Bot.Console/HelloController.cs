using System;
using Telegram.Bot.Controllers;

namespace Telegram.Bot.Console;

public class HelloController : TelegramBotControllerBase
{

    [Command("hello")]
    public async Task Hello(CancellationToken ct)
    {
        var message = "you said hello";
        await BotClient.SendMessage(
            chatId: Update.Message?.Chat.Id!,
            text: message,
            cancellationToken: ct);
    }

    [Command("/start")]
    public async Task Start(CancellationToken ct)
    {
        var message = "you said /start";

        await BotClient.SendMessage(
            chatId: Update.Message?.Chat.Id!,
            text: message,
            cancellationToken: ct);
    }
}
