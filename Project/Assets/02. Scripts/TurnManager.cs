using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [SerializeField] private List<Unit> allUnits = new List<Unit>();

    public int turnCount = 1;

    private void Init(List<Unit> units)
    {
        allUnits = units;
    }

    public void StartTurnCycle()
    {
        
    }
    
    public void NextTurn()
    {
        turnCount++;
        Log($"턴을 시작했습니다. {turnCount}턴");
    }
    
    
    void Log(string msg)
    {
        Debug.Log(msg);
    }
}
