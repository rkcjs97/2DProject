using UnityEngine;

public class CombatService
{
    public CommandResult TryMeleeAttack(Unit attacker, Unit defender)
    {
        if (attacker == null || defender == null)
            return CommandResult.Fail("공격자 또는 방어자가 없습니다.");

        MeleeAttack meleeAttack = attacker.GetComponent<MeleeAttack>();
        if (meleeAttack == null)
            return CommandResult.Fail("근접 공격 컴포넌트가 없습니다.");

        bool success = meleeAttack.TryAttack(defender);
        if (!success)
            return CommandResult.Fail("근접 공격 조건을 만족하지 않습니다.");

        return CommandResult.Ok($"{attacker.unitName} -> {defender.unitName} 공격 성공");
    }
}
