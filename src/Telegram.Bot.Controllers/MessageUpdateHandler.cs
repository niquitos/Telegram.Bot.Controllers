using System;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace Telegram.Bot.Controllers;

public class MessageUpdateHandler : IUpdateHandler
{
    private readonly IServiceProvider _serviceProvider;
    private ILogger<MessageUpdateHandler> _logger;

    public MessageUpdateHandler(IServiceProvider serviceProvider, ILogger<MessageUpdateHandler> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
    {
        _logger.LogError(exception.Message);
        return Task.CompletedTask;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var message = update.Message ?? update.EditedMessage;

        if (message is not { Text: { } messageText, From: { } user, Chat: { } chat })
            return;

        _logger.LogInformation($"Recieved command '{messageText}' from user '{user}'");

        var controllerTypes = _serviceProvider.GetServices<TelegramBotControllerBase>().Select(c => c.GetType()).ToList();

        foreach (var type in controllerTypes)
        {
            var controller = (TelegramBotControllerBase)_serviceProvider.GetRequiredService(type);
            controller.BotClient = botClient;
            controller.Update = update;

            var methods = controller.GetType().GetMethods();

            foreach (var method in methods)
            {
                var commandAttr = method.GetCustomAttributes<CommandAttribute>().FirstOrDefault();
                var patternAttr = method.GetCustomAttributes<PatternAttribute>().FirstOrDefault();

                bool shouldInvoke = false;

                if (commandAttr != null)
                {
                    var command = messageText.Split(' ')[0].ToLowerInvariant();
                    if (command == commandAttr.Command)
                        shouldInvoke = true;
                }
                else if (patternAttr != null)
                {
                    var regex = new Regex(patternAttr.Pattern);
                    if (regex.IsMatch(messageText))
                        shouldInvoke = true;
                }

                if (shouldInvoke)
                {
                    var parameters = method.GetParameters();
                    object?[]? args = null;

                    if (parameters.Length > 0 && parameters[0].ParameterType == typeof(CancellationToken))
                    {
                        args = [cancellationToken];
                    }

                    try
                    {
                        await (Task)method.Invoke(controller, args)!;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error invoking controller method.");
                    }
                    break;
                }
            }

        }
    }
}
