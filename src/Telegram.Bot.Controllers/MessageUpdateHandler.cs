using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

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

        _logger.LogInformation($"Received command '{messageText}' from user '{user}'");

        var chatId = chat.Id;
        var command = messageText.Split(' ')[0].ToLowerInvariant();
        var pathManager = _serviceProvider.GetRequiredService<ICommandPathManager>();
        var commandRegistry = _serviceProvider.GetRequiredService<CommandRegistry>();

        var botConfig = _serviceProvider.GetService<IOptions<BotConfiguration>>()?.Value
                   ?? new BotConfiguration() { CancelCommand = "cancel", CancelMessage = "Conversation closed." };

        if (command == botConfig.CancelCommand)
        {
            pathManager.ClearPath(chatId);

            if (botConfig.CancelMessage is not null)
                await botClient.SendMessage(chatId, botConfig.CancelMessage, cancellationToken: cancellationToken);

            _logger.LogInformation($"Current path: {pathManager.GetPath(chatId)}");
            return;
        }

        var currentPath = pathManager.UpdatePath(chatId, command);

        if (commandRegistry.TryGetHandler(currentPath, out var controllerType, out var methodName))
        {
            var controller = (TelegramBotControllerBase)_serviceProvider.GetRequiredService(controllerType);
            controller.BotClient = botClient;
            controller.Update = update;

            try
            {
                var method = controllerType.GetMethod(methodName)!;
                var parameters = method.GetParameters();
                object?[]? args = null;

                if (parameters.Length > 0 && parameters[0].ParameterType == typeof(CancellationToken))
                {
                    args = [cancellationToken];
                }

                await (Task)method.Invoke(controller, args)!;

                _logger.LogInformation($"Current path: {pathManager.GetPath(chatId)}");

                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error invoking controller method.");
            }
        }
        else
        {
            pathManager.RollbackLastSegment(chatId);
            var commands = commandRegistry.GetAvailableCommands(pathManager.GetPath(chatId));

            if (botConfig.SendMenuText is not null)
                await botClient.SendMessage(chatId, botConfig.SendMenuText, replyMarkup: GetMenu(commands), cancellationToken: cancellationToken);
            
            _logger.LogInformation($"Unknown command. Current path: {pathManager.GetPath(chatId)}");
        }
    }

    private static ReplyKeyboardMarkup GetMenu(IEnumerable<string> commands)
    {
        var keboardButtons = commands.Select(x => new KeyboardButton(x));

        return new([keboardButtons])
        {
            ResizeKeyboard = true,
            OneTimeKeyboard = false
        };
    }
         
}
