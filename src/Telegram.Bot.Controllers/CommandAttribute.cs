using System;

namespace Telegram.Bot.Controllers;

[AttributeUsage(AttributeTargets.Method)]
public class CommandAttribute : Attribute
{
    public string Command { get; set; }

    public CommandAttribute(string command)
    {
        Command = command.ToLowerInvariant();
    }
}
