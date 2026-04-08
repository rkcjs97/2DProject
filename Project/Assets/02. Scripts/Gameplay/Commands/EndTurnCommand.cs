public class EndTurnCommand : IGameCommand
{
    public CommandResult Execute(CommandContext context)
    {
        if (context == null || context.TurnManager == null)
            return CommandResult.Fail("TurnManager가 없습니다.");

        context.TurnManager.NextTurn();
        return CommandResult.Ok("턴 종료 명령 실행");
    }
}
