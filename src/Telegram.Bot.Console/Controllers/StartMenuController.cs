using Telegram.Bot.Controllers;

namespace Telegram.Bot.Console.Controllers;

[Command("start/menu")]
public class StartMenuController : TelegramBotControllerBase
{

    [Command] //matches start/menu
    public async Task Menu(CancellationToken ct)
    {
        var message = "you said 'start' then 'menu'";

        await BotClient.SendMessage(
            chatId: Update.Message?.Chat.Id!,
            text: message,
            cancellationToken: ct);
    }

    [Command("help")] //matches start/menu/help
    public async Task Help(CancellationToken ct)
    {
        var message = "you said 'start' then 'menu' then 'help";

        await BotClient.SendMessage(
            chatId: Update.Message?.Chat.Id!,
            text: message,
            cancellationToken: ct);
    }

}
