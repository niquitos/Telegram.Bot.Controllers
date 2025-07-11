using System;

namespace Telegram.Bot.Controllers;

public class BotConfiguration
{
    public string? CancelCommand { get; set; }
    public string? CancelMessage { get; set; }
    public string? SendMenuText { get; set; }
}
