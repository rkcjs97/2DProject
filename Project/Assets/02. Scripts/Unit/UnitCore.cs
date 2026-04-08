using System;
using UnityEngine;

public class UnitCore : MonoBehaviour
{
    [Header("Unit Info")] 
    [SerializeField] private string unitName = "Player Unit";
    [SerializeField] private int teamId = 0;

    [Header("Cached Components")] 
    private Health health;
    private MovePointHandler movePointHandler;
    private UnitSelectionVisual selectionVisual;
    
    public string UnitName => unitName;
    public int TeamId => teamId;
    
    public Health Health => health;
    public MovePointHandler MovePointHandler => movePointHandler;
    public UnitSelectionVisual SelectionVisual => selectionVisual;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        health = GetComponent<Health>();
        movePointHandler = GetComponent<MovePointHandler>();
        selectionVisual = GetComponent<UnitSelectionVisual>();
        
        if(health == null) Debug.LogError($"{name}: Health가 없습니다.");
        if(movePointHandler == null) Debug.LogError($"{name} MovePointHandler가 없습니다.");
        if(selectionVisual == null) Debug.LogError($"{name}: UnitSelectionVisual이 없습니다.");
    }
}
