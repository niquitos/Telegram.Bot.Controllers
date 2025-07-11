using System;

namespace Telegram.Bot.Controllers;

public class CommandRegistry
{
    private readonly Dictionary<string, (Type ControllerType, string MethodName)> _commands = new();

    public void RegisterCommand(string path, Type controllerType, string methodName)
    {
        _commands[path] = (controllerType, methodName);
    }

    public bool TryGetHandler(string path, out Type controllerType, out string methodName)
    {
        if (_commands.TryGetValue(path, out var entry))
        {
            controllerType = entry.ControllerType;
            methodName = entry.MethodName;
            return true;
        }

        controllerType = null!;
        methodName = null!;
        return false;
    }

    public IEnumerable<string> GetAvailableCommands(string? prefix)
    {
        var firstCommands = _commands.Keys.Select(x =>
            {
                var firstSegment = x.TrimStart('/').Split('/').FirstOrDefault();
                return firstSegment;
            })
            .OfType<string>()
            .Distinct();

        if (string.IsNullOrEmpty(prefix))
            return firstCommands;

        var matchingPaths = _commands.Keys
            .Where(path => path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            .ToArray();



        var nextSegments = matchingPaths
        .Select(path =>
        {
            var remainingPath = path[prefix.Length..].TrimStart('/');
            var firstSegment = remainingPath.Split('/').FirstOrDefault();
            return firstSegment;
        })
        .OfType<string>()
        .Distinct();

        return nextSegments!;
    }
}
