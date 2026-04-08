using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class StateMachine : MonoBehaviour
{
    private enum GameState
    {
        Title,
        City,
        InGame,
        Unit,
        Buy,
        Production
    }

    private enum Command
    {
        // Title
        Start,
        Quit,
        Back,

        // InGame
        GoCity,
        GoUnit,
        NextTurn,

        // City
        GoBuy,
        GoProduction,
        Buy,
        Production,
        Undo,

        // Unit
        Attack,
        Move
    }

    [Header("References")]
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private ProductionManager production;

    [Header("Input")]
    [SerializeField] private Key nextTurnKey = Key.Space;

    private GameState state = GameState.Title;
    private readonly List<string> logs = new List<string>();

    private int turnCost;
    private string selectedUnitName;


    private void Awake()
    {
        if (turnManager == null)
            turnManager = FindObjectOfType<TurnManager>();

        if (production == null)
            production = FindObjectOfType<ProductionManager>();

    }

    private void OnEnable()
    {
        if (turnManager != null)
            turnManager.OnTurnChanged += HandleTurnChanged;
    }

    private void OnDisable()
    {
        if (turnManager != null)
            turnManager.OnTurnChanged -= HandleTurnChanged;
    }

    private void Start()
    {
        Log("=== 게임 시작 ===");
        PrintHelp();
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        HandleBackCommand();
        HandleStateInput();
    }

    private void HandleBackCommand()
    {
        if (IsPressed(Key.B))
            ProcessCommand(Command.Back);
    }

    private void HandleStateInput()
    {
        switch (state)
        {
            case GameState.Title:
                HandleTitleInput();
                break;
            case GameState.InGame:
                HandleInGameInput();
                break;
            case GameState.City:
                HandleCityInput();
                break;
            case GameState.Buy:
                HandleBuyInput();
                break;
            case GameState.Production:
                HandleProductionInput();
                break;
            case GameState.Unit:
                HandleUnitInput();
                break;
        }
    }

    private void HandleTitleInput()
    {
        if (IsPressed(Key.S))
            ProcessCommand(Command.Start);

        if (IsPressed(Key.Q))
            ProcessCommand(Command.Quit);
    }

    private void HandleInGameInput()
    {
        if (IsPressed(nextTurnKey))
            ProcessCommand(Command.NextTurn);

        if (IsPressed(Key.I))
            ProcessCommand(Command.GoCity);

        if (IsPressed(Key.U))
            ProcessCommand(Command.GoUnit);
    }

    private void HandleCityInput()
    {
        if (IsPressed(Key.G))
            ProcessCommand(Command.GoBuy);

        if (IsPressed(Key.P))
            ProcessCommand(Command.GoProduction);
    }

    private void HandleBuyInput()
    {
        HandleNumberSelection(Command.Buy);

        if (IsPressed(Key.Z))
            ProcessCommand(Command.Undo);
    }

    private void HandleProductionInput()
    {
        HandleNumberSelection(Command.Production);
    }

    private void HandleUnitInput()
    {
        if (IsPressed(Key.A))
            ProcessCommand(Command.Attack);

        if (IsPressed(Key.M))
            ProcessCommand(Command.Move);
    }

    private void HandleNumberSelection(Command command)
    {
        if (IsPressed(Key.Digit1))
        {
            SelectUnitNameFromProductList(0);
            ProcessCommand(command);
        }

        if (IsPressed(Key.Digit2))
        {
            SelectUnitNameFromProductList(1);
            ProcessCommand(command);
        }
    }

    private void SelectUnitNameFromProductList(int index)
    {
        if (production == null)
        {
            Log("ProductionManager 참조가 없습니다.");
            selectedUnitName = null;
            return;
        }

        if (production.productList.Count <= index)
        {
            Log("해당 번호 유닛이 리스트에 없습니다.");
            selectedUnitName = null;
            return;
        }

        selectedUnitName = production.productList[index].unitName;
    }

    private void ProcessCommand(Command command)
    {
        if (command == Command.Back)
        {
            TryBack();
            return;
        }

        switch (state)
        {
            case GameState.Title: HandleTitle(command); break;
            case GameState.InGame: HandleInGame(command); break;
            case GameState.City: HandleCity(command); break;
            case GameState.Unit: HandleUnit(command); break;
            case GameState.Buy: HandleBuy(command); break;
            case GameState.Production: HandleProduction(command); break;
        }
    }

    private void HandleTitle(Command command)
    {
        if (command == Command.Start)
        {
            ChangeState(GameState.InGame, "게임을 시작합니다.");
            turnManager?.StartTurnCycle();
        }
        else if (command == Command.Quit)
        {
            Log("[Title] 게임 종료");
        }
    }

    private void HandleInGame(Command command)
    {
        if (command == Command.NextTurn)
        {
            NextTurn();
        }
        else if (command == Command.GoCity)
        {
            ChangeState(GameState.City, "도시를 관리합니다.");
            production?.PrintUnitList();
        }
        else if (command == Command.GoUnit)
        {
            ChangeState(GameState.Unit, "유닛을 관리합니다.");
        }
    }

    private void HandleCity(Command command)
    {
        if (command == Command.GoBuy)
            ChangeState(GameState.Buy, "유닛을 구입합니다.");

        if (command == Command.GoProduction)
            ChangeState(GameState.Production, "유닛을 생산합니다.");
    }

    private void HandleBuy(Command command)
    {
        if (production == null)
        {
            Log("ProductionManager 참조가 없습니다.");
            return;
        }

        if (command == Command.Buy)
        {
            if (string.IsNullOrEmpty(selectedUnitName))
            {
                Log("구매할 유닛을 먼저 선택하세요.");
                return;
            }

            BuyResult result = production.Buy(selectedUnitName);
            Log(result.Message);
            selectedUnitName = null;
        }

        if (command == Command.Undo)
        {
            UndoResult result = production.Undo();
            Log(result.Message);
        }
    }

    private void HandleProduction(Command command)
    {
        if (production == null)
        {
            Log("ProductionManager 참조가 없습니다.");
            return;
        }

        if (command == Command.Production)
        {
            if (string.IsNullOrEmpty(selectedUnitName))
            {
                Log("생산할 유닛을 먼저 선택하세요.");
                return;
            }

            ProductData data = production.Production(selectedUnitName);
            if (production.watingList.Count > 0)
                turnCost = production.watingList.Peek().Price;

            Log(data.Message);
            selectedUnitName = null;
        }
    }

    private void HandleUnit(Command command)
    {
        if (command == Command.Attack)
            Log("유닛공격");
        else if (command == Command.Move)
            Log("유닛이동");
    }

    private void NextTurn()
    {
        if (turnManager == null)
        {
            Log("TurnManager 참조가 없어 턴을 진행할 수 없습니다.");
            return;
        }

        Log($"턴을 종료합니다. {turnManager.turnCount}턴");

        turnManager.NextTurn();

        if (production == null)
            return;

        ProductResult result = production.ProductionSystem(turnCost);
        turnCost = result.TurnCost;
        Log(result.Message);
    }

    private void TryBack()
    {
        switch (state)
        {
            case GameState.InGame:
                ChangeState(GameState.Title, "타이틀로 돌아갑니다.");
                break;
            case GameState.City:
                ChangeState(GameState.InGame, "도시 관리를 종료합니다.");
                break;
            case GameState.Unit:
                ChangeState(GameState.InGame, "유닛 관리를 종료합니다.");
                break;
            case GameState.Buy:
                ChangeState(GameState.City, "유닛 구입을 종료합니다.");
                break;
            case GameState.Production:
                ChangeState(GameState.City, "유닛 생산을 종료합니다.");
                break;
        }
    }

    private void ChangeState(GameState next, string msg)
    {
        state = next;
        Log(msg);
        PrintHelp();
    }

    private void HandleTurnChanged(int turn, int team)
    {
        Log($"[StateMachine] 현재 턴: {turn}, 현재 팀: {team}");
    }

    private void PrintHelp()
    {
        if (state == GameState.Title)
        {
            Log("[Title] S: 게임시작, Q: 게임 종료");
        }
        else if (state == GameState.InGame)
        {
            Log("[InGame] Space: 다음 턴, I: 도시 관리, U: 유닛 이동, B: 타이틀로 돌아가기");
        }
        else if (state == GameState.City)
        {
            Log("[City] G: 유닛 구입, P: 유닛 생산, B: 뒤로 가기");
        }
        else if (state == GameState.Buy)
        {
            Log("[Buy] 1: 전사 구입, 2: 검사 구입, Z: 되돌리기, B: 뒤로 가기");
        }
        else if (state == GameState.Production)
        {
            Log("[Production] 1: 전사 생산, 2: 검사 생산, B: 뒤로 가기");
        }
        else if (state == GameState.Unit)
        {
            Log("[Unit] A: 공격, M: 이동, B: 뒤로가기");
        }
    }

    private static bool IsPressed(Key key)
    {
        KeyControl keyControl = Keyboard.current[key];
        return keyControl != null && keyControl.wasPressedThisFrame;
    }

    private void Log(string msg)
    {
        logs.Add(msg);
        Debug.Log(msg);
    }
}
