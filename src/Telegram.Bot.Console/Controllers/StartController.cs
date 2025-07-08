using Telegram.Bot.Controllers;

namespace Telegram.Bot.Console.Controllers;

[Command("start")]
public class StartController : TelegramBotControllerBase
{
    [Command]
    public async Task Start(CancellationToken ct) //matches start
    {
        var message = "you said 'start'";

        await BotClient.SendMessage(
            chatId: Update.Message?.Chat.Id!,
            text: message,
            cancellationToken: ct);
    }

    [Command("about", Override = true)] //ignores the base path. matches about
    public async Task About(CancellationToken ct)
    {
        var message = "you said 'about'";

        await BotClient.SendMessage(
            chatId: Update.Message?.Chat.Id!,
            text: message,
            cancellationToken: ct);
    }
}
