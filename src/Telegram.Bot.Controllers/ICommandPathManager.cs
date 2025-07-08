namespace Telegram.Bot.Controllers;

public interface ICommandPathManager
{
    string UpdatePath(long chatId, string command);
    void ClearPath(long chatId);
    string? GetPath(long chatId);

    void RollbackLastSegment(long chatId);
}
