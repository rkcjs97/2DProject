using UnityEngine;

[RequireComponent(typeof(Unit))]
public class MeleeAttack : MonoBehaviour
{
    [SerializeField] private int damage = 30;
    [SerializeField] private float attackRange = 1.1f;
    [SerializeField] private int attackCost = 1;

    private Unit owner;

    private void Awake()
    {
        owner = GetComponent<Unit>();
    }

    public bool TryAttack(Unit target)
    {
        if (owner == null || target == null)
            return false;

        if (owner == target)
            return false;

        if (!owner.CanAttack())
        {
            Debug.Log("공격 가능한 행동력이 없습니다.");
            return false;
        }

        if (owner.TeamId == target.TeamId)
        {
            Debug.Log("아군은 공격할 수 없습니다.");
            return false;
        }

        float distance = Vector3.Distance(owner.transform.position, target.transform.position);
        if (distance > attackRange)
        {
            Debug.Log("사거리가 부족합니다.");
            return false;
        }

        target.TakeDamage(damage);
        owner.ConsumeMovePoint(attackCost);

        Debug.Log($"{owner.name} -> {target.name} 근접 공격 성공");
        return true;
    }
}
