using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;

public struct ProductData
{
    public string UnitName;
    public int Price;
    public string Message;

    public ProductData(string unitName, int price, string message)
    {
        UnitName = unitName;
        Price = price;
        Message = message;
    }
}

public struct UndoResult
{
    public bool Success;
    public string Message;
    public int RemainingMoney;
    
    public UndoResult(bool success, string message, int remainingMoney)
    {
        Success = success; 
        Message = message;
        RemainingMoney = remainingMoney;
    }
}

public struct BuyResult
{
    public bool Success;
    public string Message;
    public int RemainingMoney;
    
    public BuyResult(bool success, string message, int remainingMoney)
    {
        Success = success;
        Message = message;
        RemainingMoney = remainingMoney;
    }
}

public struct ProductResult
{
    public bool Success;
    public string Message;
    public int TurnCost;
    
    public ProductResult(bool success, string message, int turnCost)
    {
        Success = success;
        Message = message;
        TurnCost = turnCost;
    }
}
public class ProductionManager : MonoBehaviour
{
    public List<Unit> productList = new List<Unit>();
    public int myMoney=100;

    public Stack<ProductData> actions = new Stack<ProductData>();
    public Queue<ProductData> watingList = new();

    public bool isEmpty = true;
    private void Start()
    {
        UnitAdd(new Warrior());
        UnitAdd(new Swordsman());
    }

    public void UnitAdd(Unit unit)
    {
        productList.Add(unit);
    }

    public ProductResult ProductionSystem(int turnCost)
    {
        string unitName;
        if (isEmpty)
        {
            if(watingList.Count <= 0)
                return new ProductResult(false, "생성할 유닛을 선택하세요", turnCost);
                
            unitName = watingList.Peek().UnitName;
            turnCost = watingList.Peek().Price;
            return new ProductResult(false, $"{unitName} 유닛 생성중... 남은 턴 : {turnCost+1}", turnCost);
        }
        if (turnCost==0)
        {
            isEmpty = true;
            if (watingList.Count >= 0)
            {
                return new ProductResult(true, "유닛 생성이 완료되었습니다.\n ",turnCost);
            }
            return new ProductResult(true, "유닛 생성이 완료되었습니다.",turnCost);
            
            
        }
        
        unitName = watingList.Peek().UnitName;
        turnCost--;
        return new ProductResult(false, $"{unitName} 유닛 생성중... 남은 턴 : {turnCost+1}",turnCost);
    }
    

    public ProductData Production(string unitName)
    {
        Unit foundUnit = productList.Find(x=> x.unitName == unitName);
        int turnCost = (foundUnit.cost/10);
        if (!isEmpty)   
        {
            watingList.Enqueue(new ProductData(foundUnit.unitName, turnCost,""));
            return new ProductData(foundUnit.unitName, turnCost,$"{foundUnit.unitName}를 {watingList.Count}번 대기열에 추가했습니다.");
        }
        isEmpty = false;
        watingList.Enqueue(new ProductData(foundUnit.unitName, turnCost,""));
        return new ProductData(foundUnit.unitName, turnCost,$"{foundUnit.unitName}의 생산을 시작합니다.");
    }
    
    public BuyResult Buy(string unitName)
    {
        Unit foundUnit = productList.Find(x => x.unitName == unitName);

        if (foundUnit == null)
        {
            return new BuyResult(false, "해당 유닛이 없습니다.", myMoney);;
        }

        if (myMoney < foundUnit.cost)
        {
            return new BuyResult(false, "금액이 부족합니다.", myMoney);;
        }

        myMoney -= foundUnit.cost;
        
        actions.Push(new ProductData(foundUnit.unitName, foundUnit.cost,""));
        
        return new BuyResult(true, $"{foundUnit.unitName} 구매 완료, 잔액: {myMoney}", myMoney);

    }

    public UndoResult Undo()
    {
        if (actions.Count > 0)
        {
            ProductData last = actions.Pop();
            myMoney += last.Price;
            return new UndoResult(true,$"{last.UnitName}유닛 환불이 완료되었습니다. 잔액: {myMoney}", myMoney);
        }
        else
        {
            return new UndoResult(false,"이전 구입 이력이 없습니다.",myMoney);
        }
    }

    public void PrintUnitList()
    {
        Debug.Log("------------------------- 유닛 리스트 -------------------------");
        foreach (Unit u in productList)
        {
            Debug.Log($"이름: {u.unitName} 비용: {u.cost} 이동력: {u.movePoint} 공격력: {u.strength} 체력: {u.hp}");
        }
        Debug.Log("--------------------------------------------------------------");
    }

}
