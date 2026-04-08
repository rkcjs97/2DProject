using UnityEngine;

public class Melee : Unit
{
    public int DefenseBonus { get; protected set; }

    protected Melee(int hp, int cost, int strength, int movePoint, string unitName, int defenseBonus)
        : base(hp, cost, strength, movePoint, unitName)
    {
        DefenseBonus = defenseBonus;
    }
}
