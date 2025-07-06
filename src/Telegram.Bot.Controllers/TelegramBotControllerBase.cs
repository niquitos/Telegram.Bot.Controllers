using Telegram.Bot.Types;

namespace Telegram.Bot.Controllers;

public abstract class TelegramBotControllerBase
{
    public ITelegramBotClient BotClient { get; set; } = null!;
    public Update Update { get; set; } = null!;
}