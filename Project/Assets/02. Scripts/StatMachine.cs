using System.Collections.Generic;
using UnityEngine;

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
        //Title
        Start,
        Quit,
        Back,
        
        //InGame
        GoCity,
        GoUnit,
        NextTurn,
        
        //City
        GoBuy,
        GoProduction,
        Buy,
        Production,
        Undo,
        
        //Unit
        Attack,
        Move
    }
    
    [Header("Ref")]
    [SerializeField] public TurnManager turnManager;
    
    [Header("Space로 다음턴")]
    [SerializeField] KeyCode nextTurnKey = KeyCode.Space;
    [SerializeField] private ProductionManager production;
    
    GameState state = GameState.Title;
    List<string> logs = new();
    
    public int turnCost;
    private string uname;
    private Command cmd;

    void Start()
    {
        Debug.Log("=== 게임 시작 ===");
        PrintHelp();
    }
    
    void Update()
    {
        //Title
        if (Input.GetKeyDown(KeyCode.S))
        {
            cmd = Command.Start;
            ProcessCommand(cmd);
        }
    
        if (Input.GetKeyDown(KeyCode.Q))
        {
            cmd = Command.Quit;
            ProcessCommand(cmd);
        }
        
        //InGame
        if (Input.GetKeyDown(nextTurnKey))
        {
            cmd = Command.NextTurn;
            ProcessCommand(cmd);
        }
       
        if (Input.GetKeyDown(KeyCode.I))
        {
            cmd = Command.GoCity;
            ProcessCommand(cmd);
        }
    
        if (Input.GetKeyDown(KeyCode.U))
        {
            cmd = Command.GoUnit;
            ProcessCommand(cmd);
        }
        
        //City
        
        //GoBuy
        if (Input.GetKeyDown(KeyCode.G))
        {
            cmd = Command.GoBuy;
            ProcessCommand(cmd);
        }
        
        //GoProduction
        if (Input.GetKeyDown(KeyCode.P))
        {
            cmd = Command.GoProduction;
            ProcessCommand(cmd);
        }
        
        //Buy
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            uname = production.productList[0].unitName;
            cmd = Command.Buy;
            ProcessCommand(cmd);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            uname = production.productList[1].unitName;
            cmd = Command.Buy;
            ProcessCommand(cmd);
        }
        
        //Production
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            uname = production.productList[0].unitName;
            cmd = Command.Production;
            ProcessCommand(cmd);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            uname = production.productList[1].unitName;
            cmd = Command.Production;
            ProcessCommand(cmd);
        }
        
        //Undo
        if (Input.GetKeyDown(KeyCode.Z))
        {
            cmd = Command.Undo;
            ProcessCommand(cmd);
        }
        
        //Unit
        if (Input.GetKeyDown(KeyCode.A))
        {
            cmd =  Command.Attack;
            ProcessCommand(cmd);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            cmd = Command.Move;
            ProcessCommand(cmd);
        }
        
        //Back
        if (Input.GetKeyDown(KeyCode.B))
        {
            cmd = Command.Back;
            ProcessCommand(cmd);
        }
        
    }

    void NextTurn()
    {
        Log($"턴을 종료합니다. {turnManager.turnCount}턴");
        ProductResult result = production.ProductionSystem(turnCost);
        turnCost = result.TurnCost;
        Log(result.Message);
        
        
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
            ChangeState(GameState.InGame,"게임을 시작합니다.");
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
            ChangeState(GameState.City,"도시를 관리합니다.");
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
        {
            ChangeState(GameState.Buy, "유닛을 구입합니다.");
        }

        if (cmd == Command.GoProduction)
        {
            ChangeState(GameState.Production, "유닛을 생산합니다.");
        }
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
            ProductData data =  production.Production(uname);
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
            ChangeState(GameState.Title,"타이틀로 돌아갑니다.");
        }else if (state == GameState.City)
        {
            ChangeState(GameState.InGame,"도시 관리를 종료합니다.");
        }else if (state == GameState.Unit)
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

    void PrintHelp()
    {

        if (state == GameState.Title)
        {
            Log("[Title] S: 게임시작, Q: 게임 종료");
        }
        else if (state == GameState.InGame)
        {
            Log("[InGame] I: 도시 관리, U: 유닛 이동, B: 타이틀로 돌아가기");
        }
        else if (state == GameState.City)
        {
            Log("[City] G: 유닛 구입, P: 유닛 생산, B: 뒤로 가기");
        }
        else if (state == GameState.Buy)
        {
            Log("[Buy] 1: 전사 구입, 2: 검사 구입, B: 뒤로 가기");
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
