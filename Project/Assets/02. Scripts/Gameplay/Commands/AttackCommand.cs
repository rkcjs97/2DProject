public class AttackCommand : IGameCommand
{
    private readonly Unit attacker;
    private readonly Unit defender;

    public AttackCommand(Unit attacker, Unit defender)
    {
        this.attacker = attacker;
        this.defender = defender;
    }

    public CommandResult Execute(CommandContext context)
    {
        if (context == null || context.CombatService == null)
            return CommandResult.Fail("CombatService가 없습니다.");

        return context.CombatService.TryMeleeAttack(attacker, defender);
    }
}
