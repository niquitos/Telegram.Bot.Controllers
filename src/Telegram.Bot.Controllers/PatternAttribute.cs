namespace Telegram.Bot.Controllers;

[AttributeUsage(AttributeTargets.Method)]
public class PatternAttribute : Attribute
{
    public string Pattern { get; set; }

    public PatternAttribute(string pattern)
    {
        Pattern = pattern;
    }
}
