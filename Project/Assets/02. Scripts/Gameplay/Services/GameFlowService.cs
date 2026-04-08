using UnityEngine;

public class GameFlowService
{
    public CommandResult Execute(IGameCommand command, CommandContext context)
    {
        if (command == null)
            return CommandResult.Fail("실행할 커맨드가 없습니다.");

        CommandResult result = command.Execute(context);
        if (!string.IsNullOrEmpty(result.Message))
            Debug.Log($"[Command] {result.Message}");

        return result;
    }
}
