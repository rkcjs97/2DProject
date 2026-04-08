using UnityEngine;

public class Warrior: Melee
{
    public Warrior()
        : base(hp: 100, cost: 40, strength: 10, movePoint: 2, unitName: "전사", defenseBonus: 0)
    {
        
    }
   
}
