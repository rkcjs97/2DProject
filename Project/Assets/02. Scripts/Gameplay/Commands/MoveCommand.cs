using UnityEngine;

public class MoveCommand : IGameCommand
{
    private readonly Unit actor;
    private readonly Vector3 targetPos;

    public MoveCommand(Unit actor, Vector3 targetPos)
    {
        this.actor = actor;
        this.targetPos = targetPos;
    }

    public CommandResult Execute(CommandContext context)
    {
        if (actor == null)
            return CommandResult.Fail("이동할 유닛이 없습니다.");

        if (!actor.CanMove())
            return CommandResult.Fail("이동할 수 없는 상태입니다.");

        actor.SetMoveTarget(targetPos);
        return CommandResult.Ok($"{actor.unitName} 이동 명령 실행");
    }
}
