using System;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [Header("Turn State")]
    [SerializeField] private List<Unit> allUnits = new List<Unit>();
    [SerializeField] private int currentTeamTurn = 0;

    [Header("References")]
    [SerializeField] private UnitSelectionManager unitSelectionManager;

    public int turnCount { get; private set; } = 1;
    public int CurrentTeamTurn => currentTeamTurn;
    public TurnPhase CurrentPhase { get; private set; } = TurnPhase.Movement;

    public event Action<int, int> OnTurnChanged;

    private void Awake()
    {
        if (unitSelectionManager == null)
            unitSelectionManager = FindObjectOfType<UnitSelectionManager>();

        RefreshUnitList();
    }

    private void Start()
    {
        StartTurnCycle();
    }

    public void StartTurnCycle()
    {
        RefreshUnitList();
        ResetCurrentTeamUnits();

        CurrentPhase = TurnPhase.Movement;
        Log($"게임 시작 - {turnCount}턴 / 팀 {currentTeamTurn} 행동 시작");
        OnTurnChanged?.Invoke(turnCount, currentTeamTurn);
    }

    public void NextTurn()
    {
        RefreshUnitList();
        RemoveNullUnits();

        int previousTeam = currentTeamTurn;
        currentTeamTurn = GetNextTeamId(currentTeamTurn);

        if (currentTeamTurn <= previousTeam)
            turnCount++;

        ResetCurrentTeamUnits();
        ClearInvalidSelection();

        CurrentPhase = TurnPhase.Movement;
        Log($"턴 진행 -> {turnCount}턴 / 팀 {currentTeamTurn} 행동 시작");
        OnTurnChanged?.Invoke(turnCount, currentTeamTurn);
    }

    public void RefreshUnitList()
    {
        allUnits = new List<Unit>(FindObjectsOfType<Unit>());
        RemoveNullUnits();
    }


    public void SetPhase(TurnPhase phase)
    {
        CurrentPhase = phase;
        Log($"턴 페이즈 변경 -> {CurrentPhase}");
    }

    private int GetNextTeamId(int fromTeam)
    {
        HashSet<int> teams = new HashSet<int>();
        foreach (Unit unit in allUnits)
        {
            if (unit != null)
                teams.Add(unit.TeamId);
        }

        if (teams.Count == 0)
            return 0;

        int candidate = fromTeam + 1;
        for (int i = 0; i < 100; i++)
        {
            if (teams.Contains(candidate))
                return candidate;

            candidate++;
            if (candidate > 32)
                candidate = 0;
        }

        foreach (int team in teams)
            return team;

        return 0;
    }

    private void ResetCurrentTeamUnits()
    {
        foreach (Unit unit in allUnits)
        {
            if (unit == null)
                continue;

            if (unit.TeamId == currentTeamTurn)
                unit.ResetMove();
        }
    }

    private void ClearInvalidSelection()
    {
        if (unitSelectionManager == null)
            return;

        Unit selected = unitSelectionManager.SelectedUnit;
        if (selected == null)
            return;

        if (selected.TeamId != currentTeamTurn)
            unitSelectionManager.DeselectUnit();
    }

    private void RemoveNullUnits()
    {
        allUnits.RemoveAll(unit => unit == null);
    }

    private void Log(string msg)
    {
        Debug.Log(msg);
    }
}
