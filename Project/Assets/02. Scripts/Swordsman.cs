public class Swordsman : Melee
{
    protected override UnitStats GetBaseStats()
    {
        return new UnitStats("검사", 200, 60, 20, 2);
    }
}
