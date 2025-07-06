using Telegram.Bot.Types.ReplyMarkups;

namespace Telegram.Bot.Controllers;

public interface IMessageService
{
    Task SendMessageAsync(long chatId, string message, CancellationToken ct, bool sendMenuButtons = true);
}

