using Telegram.Bot.Controllers;

namespace Telegram.Bot.Console.Controllers;

[Command("start/menu/help")]
public class StartMenuHelpController : TelegramBotControllerBase
{

    [Command] //matches start/menu/help
    public async Task Menu(CancellationToken ct)
    {
        var message = "you said 'start' then 'menu' then 'help";

        await MessageService.SendMessageAsync(Update.Message!.Chat.Id!,message, ct);
    }

    [Command("contact")] //matches start/menu/help
    public async Task Help(CancellationToken ct)
    {
        var message = "you said 'start' then 'menu' then 'help' then 'contact'";

        await MessageService.SendMessageAsync(Update.Message!.Chat.Id!,message, ct);
    }

}

