using UnityEngine;

public class Unit : MonoBehaviour
{
    [Header("Selection Visual")]
    [SerializeField] private GameObject selectedIndicator;

    [Header("Core Stats")]
    [SerializeField] private string defaultUnitName = "Unit";
    [SerializeField] private int defaultTeamId = 0;
    [SerializeField] private int defaultHp = 100;
    [SerializeField] private int defaultCost = 40;
    [SerializeField] private int defaultStrength = 10;
    [SerializeField] private int defaultMovePoint = 2;

    public int hp { get; protected set; }
    public int cost { get; protected set; }
    public int strength { get; protected set; }
    public int movePoint { get; protected set; }
    public int currentMovePoint { get; protected set; }
    public string unitName { get; protected set; }

    public int TeamId => defaultTeamId;
    public float moveSpeed = 5f;

    private Vector3 targetPosition;
    private bool isMoving;

    protected virtual void Awake()
    {
        hp = defaultHp;
        cost = defaultCost;
        strength = defaultStrength;
        movePoint = defaultMovePoint;
        unitName = defaultUnitName;
    }

    public void Start()
    {
        currentMovePoint = movePoint;
        targetPosition = transform.position;
    }

    public void Update()
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

    public void TakeDamage(int damage)
    {
        if (hp <= 0)
            return;

        hp = Mathf.Max(0, hp - Mathf.Max(0, damage));
        Debug.Log($"{unitName} 피해: {damage}, 남은 HP: {hp}");

        if (hp == 0)
            Die();
    }

    private void Die()
    {
        Debug.Log($"{unitName} 유닛이 제거되었습니다.");
        Destroy(gameObject);
    }

    public void ResetMove()
    {
        currentMovePoint = movePoint;
    }
}

public class Swordsman : Melee
{
}
