using Telegram.Bot.Types;

namespace Telegram.Bot.Controllers;

public abstract class TelegramBotControllerBase
{
    public IMessageService MessageService { get; set; } = null!;
    public Update Update { get; set; } = null!;
}