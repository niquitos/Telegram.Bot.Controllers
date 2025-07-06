namespace Telegram.Bot.Controllers;

public record CommandAttributeDetatails(CommandAttribute Attribute, Type ControllerType, string MethodName)
{
  
}
