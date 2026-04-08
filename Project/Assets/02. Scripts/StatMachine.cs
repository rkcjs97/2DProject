using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class StateMachine : MonoBehaviour
{
    enum GameState
    {
        Title,
        City,
        InGame,
        Unit,
        Buy,
        Production
    }

    enum Command
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

    [Header("Ref")]
    [SerializeField] public TurnManager turnManager;

    [Header("Space로 다음턴")]
    [SerializeField] private Key nextTurnKey = Key.Space;
    [SerializeField] private ProductionManager production;

    private GameState state = GameState.Title;
    private readonly List<string> logs = new List<string>();

    public int turnCost;
    private string uname;
    private Command cmd;

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

    void Start()
    {
        Debug.Log("=== 게임 시작 ===");
        PrintHelp();
    }

    void Update()
    {
        if (Keyboard.current == null)
            return;

        // Title
        if (IsPressed(Key.S))
        {
            cmd = Command.Start;
            ProcessCommand(cmd);
        }

        if (IsPressed(Key.Q))
        {
            cmd = Command.Quit;
            ProcessCommand(cmd);
        }

        // InGame
        if (IsPressed(nextTurnKey))
        {
            cmd = Command.NextTurn;
            ProcessCommand(cmd);
        }

        if (IsPressed(Key.I))
        {
            cmd = Command.GoCity;
            ProcessCommand(cmd);
        }

        if (IsPressed(Key.U))
        {
            cmd = Command.GoUnit;
            ProcessCommand(cmd);
        }

        // City
        if (IsPressed(Key.G))
        {
            cmd = Command.GoBuy;
            ProcessCommand(cmd);
        }

        if (IsPressed(Key.P))
        {
            cmd = Command.GoProduction;
            ProcessCommand(cmd);
        }

        // Buy / Production number key routing
        if (IsPressed(Key.Digit1))
            HandleNumberSelection(0);

        if (IsPressed(Key.Digit2))
            HandleNumberSelection(1);

        // Undo
        if (IsPressed(Key.Z))
        {
            cmd = Command.Undo;
            ProcessCommand(cmd);
        }

        // Unit
        if (IsPressed(Key.A))
        {
            cmd = Command.Attack;
            ProcessCommand(cmd);
        }

        if (IsPressed(Key.M))
        {
            cmd = Command.Move;
            ProcessCommand(cmd);
        }

        // Back
        if (IsPressed(Key.B))
        {
            cmd = Command.Back;
            ProcessCommand(cmd);
        }
    }

    private static bool IsPressed(Key key)
    {
        KeyControl keyControl = Keyboard.current[key];
        return keyControl != null && keyControl.wasPressedThisFrame;
    }

    void NextTurn()
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

    private void HandleNumberSelection(int index)
    {
        if (production == null)
        {
            Log("ProductionManager 참조가 없습니다.");
            return;
        }

        if (production.productList.Count <= index)
        {
            Log("해당 번호 유닛이 리스트에 없습니다.");
            return;
        }

        uname = production.productList[index].unitName;

        if (state == GameState.Buy)
            cmd = Command.Buy;
        else if (state == GameState.Production)
            cmd = Command.Production;
        else
            return;

        ProcessCommand(cmd);
    }

    void ProcessCommand(Command cmd)
    {
        if (cmd == Command.Back)
        {
            TryBack();
            return;
        }

        switch (state)
        {
            case GameState.Title: HandleTitle(cmd); break;
            case GameState.InGame: HandleInGame(cmd); break;
            case GameState.City: HandleCity(cmd); break;
            case GameState.Unit: HandleUnit(cmd); break;
            case GameState.Buy: HandleBuy(cmd); break;
            case GameState.Production: HandleProduction(cmd); break;
        }
    }

    void HandleTitle(Command cmd)
    {
        if (cmd == Command.Start)
        {
            ChangeState(GameState.InGame, "게임을 시작합니다.");
            if (turnManager != null)
                turnManager.StartTurnCycle();
        }
        else if (cmd == Command.Quit)
            Log("[Title] 게임 종료");
    }

    void HandleInGame(Command cmd)
    {
        if (cmd == Command.NextTurn)
        {
            NextTurn();
        }
        else if (cmd == Command.GoCity)
        {
            ChangeState(GameState.City, "도시를 관리합니다.");
            if (production != null)
                production.PrintUnitList();
        }
        else if (cmd == Command.GoUnit)
        {
            ChangeState(GameState.Unit, "유닛을 관리합니다.");
        }
    }

    void HandleCity(Command cmd)
    {
        if (cmd == Command.GoBuy)
            ChangeState(GameState.Buy, "유닛을 구입합니다.");

        if (cmd == Command.GoProduction)
            ChangeState(GameState.Production, "유닛을 생산합니다.");
    }

    void HandleBuy(Command cmd)
    {
        if (cmd == Command.Buy)
        {
            BuyResult result = production.Buy(uname);
            Log(result.Message);
            uname = null;
        }

        if (cmd == Command.Undo)
        {
            UndoResult result = production.Undo();
            Log(result.Message);
        }
    }

    void HandleProduction(Command cmd)
    {
        if (cmd == Command.Production)
        {
            ProductData data = production.Production(uname);
            turnCost = production.watingList.Peek().Price;
            Log(data.Message);
            uname = null;
        }
    }

    void HandleUnit(Command cmd)
    {
        if (cmd == Command.Attack)
        {
            Log("유닛공격");
        }
        else if (cmd == Command.Move)
        {
            Log("유닛이동");
        }
    }

    void TryBack()
    {
        if (state == GameState.InGame)
        {
            ChangeState(GameState.Title, "타이틀로 돌아갑니다.");
        }
        else if (state == GameState.City)
        {
            ChangeState(GameState.InGame, "도시 관리를 종료합니다.");
        }
        else if (state == GameState.Unit)
        {
            ChangeState(GameState.InGame, "유닛 관리를 종료합니다.");
        }
        else if (state == GameState.Buy)
        {
            ChangeState(GameState.City, " 유닛 구입을 종료합니다.");
        }
        else if (state == GameState.Production)
        {
            ChangeState(GameState.City, " 유닛 생산을 종료합니다.");
        }
    }

    void ChangeState(GameState next, string msg)
    {
        Log(msg);
        state = next;

        PrintHelp();
    }

    private void HandleTurnChanged(int turn, int team)
    {
        Log($"[StateMachine] 현재 턴: {turn}, 현재 팀: {team}");
    }

    void PrintHelp()
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

    void Log(string msg)
    {
        logs.Add(msg);
        Debug.Log(msg);
    }
}
