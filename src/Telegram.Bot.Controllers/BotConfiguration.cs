using System;

namespace Telegram.Bot.Controllers;

public class BotConfiguration
{
    public string? CancelCommand { get; private set; }
    public string? CancelText { get; private set; }
    public string? CommandNotFoundText { get; private set; }
    public string? ConversationClosedText { get; private set; }

    public static BotConfiguration Default => new() { CancelCommand = "cancel", CancelText = "Conversation closed." };

    public void SetCommandNotFoundText(string text) => CommandNotFoundText = text;
    public void SetConversationClosedText(string text) => ConversationClosedText = text;

    public void SetCancellation(string cancelCommand, string cancelText)
    {
        CancelCommand = cancelCommand;
        CancelText = cancelText;
    }
}