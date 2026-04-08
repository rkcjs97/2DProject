public class Warrior : Melee
{
    protected override UnitStats GetBaseStats()
    {
        return new UnitStats("전사", 100, 40, 12, 2);
    }
}
