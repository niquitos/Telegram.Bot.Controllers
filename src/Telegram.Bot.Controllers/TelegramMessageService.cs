using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegram.Bot.Controllers;

public class TelegramMessageService : IMessageService
{
    private readonly ITelegramBotClient _botClient;
    private readonly CommandRegistry _commandRegistry;
    private readonly ICommandPathManager _pathManager;
    private readonly ILogger<TelegramMessageService> _logger;
    private readonly BotConfiguration _settings;

    public TelegramMessageService(ITelegramBotClient botClient,
    CommandRegistry commandRegistry,
    IOptions<BotConfiguration> cofigs,
    ICommandPathManager pathManager, ILogger<TelegramMessageService> logger)
    {
        _botClient = botClient;
        _commandRegistry = commandRegistry;
        _pathManager = pathManager;
        _logger = logger;
        _settings = cofigs?.Value ?? BotConfiguration.Default;
    }

    public async Task SendMessageAsync(long chatId, string message, CancellationToken ct, bool sendMenuButtons = true)
    {
        var rootCommands = false;
        var nextCommands = _commandRegistry.GetAvailableCommands(_pathManager.GetPath(chatId), _settings).ToList();
        if (nextCommands.Count == 0)
        {
            rootCommands = true;

            nextCommands = [.. _commandRegistry.GetRootCommands()];
            _pathManager.ClearPath(chatId);

            _logger.LogInformation($"Current path: {_pathManager.GetPath(chatId)}");
        }

        nextCommands = [.. nextCommands, .. _commandRegistry.GetOverrideCommands()];

        if (_settings.CancelCommand is not null)
            nextCommands.Add(_settings.CancelCommand!);

        await _botClient.SendMessage(chatId: chatId, text: message, replyMarkup: sendMenuButtons ? CreateButtons(nextCommands) : null, cancellationToken: ct);

        if (rootCommands && _settings.ConversationClosedText is not null)
        {
            await _botClient.SendMessage(chatId: chatId, text: _settings.ConversationClosedText, replyMarkup: sendMenuButtons ? CreateButtons(nextCommands) : null, cancellationToken: ct);
        }
    }

    private static ReplyKeyboardMarkup? CreateButtons(List<string> commands)
    {
        if (commands.Count == 0)
            return null;

        var keyboardButtons = new List<List<KeyboardButton>>();
        List<KeyboardButton>? currentRow = null;

        foreach (var command in commands.Distinct())
        {
            if (currentRow == null || currentRow.Count >= 3)
            {
                currentRow = [];
                keyboardButtons.Add(currentRow);
            }

            currentRow.Add(command);
        }

        return new(keyboardButtons)
        {
            ResizeKeyboard = true,
            OneTimeKeyboard = false
        };
    }
}

