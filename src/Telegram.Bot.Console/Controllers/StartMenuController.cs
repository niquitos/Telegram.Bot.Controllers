using Telegram.Bot.Controllers;

namespace Telegram.Bot.Console.Controllers;

[Command("start/menu")]
public class StartMenuController : TelegramBotControllerBase
{

    [Command] //matches start/menu
    public async Task Menu(CancellationToken ct)
    {
        var message = "you said 'start' then 'menu'";

        await MessageService.SendMessageAsync(Update.Message!.Chat.Id!,message, ct);
    }

    [Command("item")] //matches start/menu/item
    public async Task MenuItem(CancellationToken ct)
    {
        var message = "you said 'start' then 'menu' then 'item";

        await MessageService.SendMessageAsync(Update.Message!.Chat.Id!,message, ct);
    }

}

