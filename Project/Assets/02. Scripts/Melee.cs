using UnityEngine;

public class Melee : Unit
{
    [Header("Melee")]
    [SerializeField] private int defenseBonus = 0;
    [SerializeField] private int meleeAttackBonus = 0;

    public int DefenseBonus => defenseBonus;

    public override int GetAttackDamage()
    {
        return Mathf.Max(0, base.GetAttackDamage() + meleeAttackBonus);
    }

    public override void TakeDamage(int damage)
    {
        int reducedDamage = Mathf.Max(0, damage - defenseBonus);
        base.TakeDamage(reducedDamage);
    }
}
