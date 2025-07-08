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
}
