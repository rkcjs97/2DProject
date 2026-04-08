using UnityEngine;

public class Melee : Unit
{
    [SerializeField] private int defenseBonus = 0;
    public int DefenseBonus => defenseBonus;
}
