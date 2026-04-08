using UnityEngine;

public class Unit : MonoBehaviour
{
    [Header("Selection Visual")]
    [SerializeField] private GameObject selectedIndicator;
    
    public int hp { get; protected set; }
    public int cost { get; protected set; }

    public int strength { get; protected set; }
    public int movePoint { get; protected set; }
    public int currentMovePoint { get; protected set; }
    public string unitName { get; protected set; }
    
    protected Unit(int hp, int cost, int strength, int movePoint, string unitName)
    {
        this.hp = hp;
        this.cost = cost;
        this.strength = strength;
        this.movePoint = movePoint;
        this.unitName = unitName;
    }
    
    public float moveSpeed = 5f;
    
    private Vector3 targetPosition;
    private bool isMoving;

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
        if(selectedIndicator != null)
            selectedIndicator.SetActive(isSelected);
    }

    private void MoveToTarget()
    {
        if(!isMoving)
            return;
        transform.position = Vector3.MoveTowards(
            transform.position, 
            targetPosition,
            moveSpeed*Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            transform.position = targetPosition;
            isMoving = false;
        }
        
    }
    
    public bool CanMove()
    {
        return currentMovePoint > 0 && !isMoving;
    }
    
    public void ResetMove()
    {
        currentMovePoint = movePoint;
    }
}

public class Swordsman : Melee
{
    public Swordsman()
        : base(hp: 200, cost: 60, strength: 20, movePoint: 2, unitName: "검사", defenseBonus: 0)
    {
        
    }
}
