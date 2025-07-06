using System;

namespace Telegram.Bot.Controllers;

public class CommandRegistry
{
    public readonly Dictionary<string, CommandAttributeDetatails> Commands = new();

    public IEnumerable<string> GetAvailableCommands(string? prefix, BotConfiguration botConfig)
    {
        if (string.IsNullOrWhiteSpace(prefix))
            return [];

        var matchingCommands = Commands.Keys
            .Where(path => path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));

        var nextSegments = matchingCommands
        .Select(path =>
        {
            var remainingPath = path[prefix.Length..].TrimStart('/');
            return remainingPath.Split('/')[0];
        })
        .Where(x => !string.IsNullOrEmpty(x));

        return nextSegments;
    }

    public IEnumerable<string> GetRootCommands()
    {
        var rootCommands = Commands.Keys.Select(x => x.Split('/')[0])
            .Where(x => !string.IsNullOrWhiteSpace(x));

        return rootCommands;
    }

    public IEnumerable<string> GetOverrideCommands()
    {
        var overrideCommands = Commands.Where(x => x.Value.Attribute.Override)
            .Select(x => x.Key.Split('/')[0])
            .Where(x => !string.IsNullOrWhiteSpace(x));

        return overrideCommands;
    }
}
