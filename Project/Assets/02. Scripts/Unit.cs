using UnityEngine;

[System.Serializable]
public struct UnitStats
{
    public string unitName;
    public int maxHp;
    public int cost;
    public int strength;
    public int movePoint;

    public UnitStats(string unitName, int maxHp, int cost, int strength, int movePoint)
    {
        this.unitName = unitName;
        this.maxHp = maxHp;
        this.cost = cost;
        this.strength = strength;
        this.movePoint = movePoint;
    }
}

public class Unit : MonoBehaviour
{
    [Header("Selection")]
    [SerializeField] private GameObject selectedIndicator;

    [Header("Team / Movement")]
    [SerializeField] private int teamId = 0;
    [SerializeField] private float moveSpeed = 5f;

    [Header("Optional Override Stats")]
    [SerializeField] private bool useOverrideStats;
    [SerializeField] private UnitStats overrideStats = new UnitStats("Unit", 100, 40, 10, 2);

    public int hp { get; protected set; }
    public int cost { get; protected set; }
    public int strength { get; protected set; }
    public int movePoint { get; protected set; }
    public int currentMovePoint { get; protected set; }
    public string unitName { get; protected set; }

    public int TeamId => teamId;

    private Vector3 targetPosition;
    private bool isMoving;

    protected virtual UnitStats GetBaseStats()
    {
        return new UnitStats("Unit", 100, 40, 10, 2);
    }

    protected virtual void Awake()
    {
        UnitStats finalStats = useOverrideStats ? overrideStats : GetBaseStats();

        unitName = finalStats.unitName;
        hp = Mathf.Max(1, finalStats.maxHp);
        cost = Mathf.Max(0, finalStats.cost);
        strength = Mathf.Max(0, finalStats.strength);
        movePoint = Mathf.Max(0, finalStats.movePoint);
    }

    private void Start()
    {
        currentMovePoint = movePoint;
        targetPosition = transform.position;
    }

    private void Update()
    {
        MoveToTarget();
    }

    public void SetMoveTarget(Vector3 targetPos)
    {
        if (!CanMove())
        {
            Debug.Log("이동 횟수 부족");
            return;
        }

        targetPosition = targetPos;
        targetPosition.z = 0f;
        currentMovePoint--;
        isMoving = true;
    }

    public void SetSelected(bool isSelected)
    {
        if (selectedIndicator != null)
            selectedIndicator.SetActive(isSelected);
    }

    public bool CanMove()
    {
        return currentMovePoint > 0 && !isMoving && hp > 0;
    }

    public bool CanAttack()
    {
        return currentMovePoint > 0 && !isMoving && hp > 0;
    }

    public void ConsumeMovePoint(int amount = 1)
    {
        currentMovePoint = Mathf.Max(0, currentMovePoint - amount);
    }

    public virtual int GetAttackDamage()
    {
        return strength;
    }

    public virtual void TakeDamage(int damage)
    {
        if (hp <= 0)
            return;

        hp = Mathf.Max(0, hp - Mathf.Max(0, damage));
        Debug.Log($"{unitName} 피해: {damage}, 남은 HP: {hp}");

        if (hp == 0)
            Die();
    }

    public void ResetMove()
    {
        currentMovePoint = movePoint;
    }

    private void MoveToTarget()
    {
        if (!isMoving)
            return;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            transform.position = targetPosition;
            isMoving = false;
        }
    }

    private void Die()
    {
        Debug.Log($"{unitName} 유닛이 제거되었습니다.");
        Destroy(gameObject);
    }
}
