using Telegram.Bot.Controllers;

namespace Telegram.Bot.Console.Controllers;

[Command("start")]
public class StartController : TelegramBotControllerBase
{
    [Command]
    public async Task Start(CancellationToken ct) //matches start
    {
        var message = "you said 'start'";

        await MessageService.SendMessageAsync(Update.Message!.Chat.Id!,message, ct);
    }

    [Command("about", Override = true)] //ignores the base path. matches about
    public async Task About(CancellationToken ct)
    {
        var message = "you said 'about'";

        await MessageService.SendMessageAsync(Update.Message!.Chat.Id!,message, ct);
    }
}
