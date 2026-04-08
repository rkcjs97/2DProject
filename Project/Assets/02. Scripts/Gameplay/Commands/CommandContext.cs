public sealed class CommandContext
{
    public TurnManager TurnManager { get; }
    public CombatService CombatService { get; }

    public CommandContext(TurnManager turnManager, CombatService combatService)
    {
        TurnManager = turnManager;
        CombatService = combatService;
    }
}
