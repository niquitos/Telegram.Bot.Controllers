namespace Telegram.Bot.Controllers;

public class InMemoryCommandPathManager : ICommandPathManager
{
    private readonly Dictionary<long, string> _currentPaths = new();

    public string UpdatePath(long chatId, string command)
    {
        command = command.ToLowerInvariant();

        if (_currentPaths.TryGetValue(chatId, out var currentPath))
        {
            // Append the next step
            _currentPaths[chatId] = $"{currentPath}/{command}";
        }
        else
        {
            // Start new path
            _currentPaths[chatId] = command;
        }

        return _currentPaths[chatId];
    }

    public void ClearPath(long chatId)
    {
        _currentPaths.Remove(chatId);
    }

    public string? GetPath(long chatId)
    {
        _currentPaths.TryGetValue(chatId, out var path);
        return path;
    }

    public void RollbackLastSegment(long chatId)
    {
        if (_currentPaths.TryGetValue(chatId, out var path))
        {
            var segments = path.Split('/');
            if (segments.Length > 1)
            {
                _currentPaths[chatId] = string.Join("/", segments[..^1]);
            }
            else
            {
                // Only one segment left — remove it
                _currentPaths.Remove(chatId);
            }
        }
    }
}