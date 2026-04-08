public interface IGameCommand
{
    CommandResult Execute(CommandContext context);
}
