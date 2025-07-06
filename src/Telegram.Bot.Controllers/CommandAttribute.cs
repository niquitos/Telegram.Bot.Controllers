namespace Telegram.Bot.Controllers;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class CommandAttribute : Attribute
{
    public string? Path { get; set; }
    public bool Override { get; set; }

    public CommandAttribute(string? path = null, bool @override = false)
    {
        Path = path?.Trim().ToLowerInvariant();
        Override = @override;
    }
}
