using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class UnitMoveManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Tilemap moveTilemap;
    [SerializeField] private LayerMask unitLayerMask;
    [SerializeField] private UnitSelectionManager unitSelectionManager;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (unitSelectionManager == null)
            unitSelectionManager = FindObjectOfType<UnitSelectionManager>();
    }

    private void Update()
    {
        HandleMouseInput();
    }

    private void HandleMouseInput()
    {
        if (Mouse.current == null)
            return;

        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();

        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(
            new Vector3(mouseScreenPos.x, mouseScreenPos.y, -mainCamera.transform.position.z)
        );
        mouseWorldPos.z = 0f;

        // 1. 유닛 클릭
        Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos, unitLayerMask);
        if (hit != null)
        {
            Unit clickedUnit = hit.GetComponent<Unit>();
            if (clickedUnit != null)
            {
                HandleUnitClick(clickedUnit);
                return;
            }
        }

        // 2. 현재 선택 유닛 없으면 종료
        if (unitSelectionManager.SelectedUnit == null)
            return;

        // 3. 타일 클릭 여부 확인
        Vector3Int clickedCell = moveTilemap.WorldToCell(mouseWorldPos);
        bool hasTile = moveTilemap.HasTile(clickedCell);

        if (!hasTile)
        {
            unitSelectionManager.DeselectUnit();
            return;
        }

        // 4. 이동 가능 확인
        if (!unitSelectionManager.SelectedUnit.CanMove())
        {
            Debug.Log("이동 횟수를 모두 소진했습니다.");
            return;
        }

        Vector3 targetPos = moveTilemap.CellToWorld(clickedCell);
        targetPos.z = 0f;

        unitSelectionManager.SelectedUnit.SetMoveTarget(targetPos);

        Debug.Log($"{unitSelectionManager.SelectedUnit.unitName} 이동 -> {clickedCell}");
    }

    private void HandleUnitClick(Unit clickedUnit)
    {
        Unit selected = unitSelectionManager.SelectedUnit;

        if (selected == null)
        {
            unitSelectionManager.SelectUnit(clickedUnit);
            return;
        }

        if (selected == clickedUnit)
        {
            unitSelectionManager.SelectUnit(clickedUnit);
            return;
        }

        if (selected.TeamId == clickedUnit.TeamId)
        {
            unitSelectionManager.SelectUnit(clickedUnit);
            return;
        }

        MeleeAttack attack = selected.GetComponent<MeleeAttack>();
        if (attack == null)
        {
            Debug.Log("선택된 유닛은 공격 기능이 없습니다.");
            return;
        }

        bool attacked = attack.TryAttack(clickedUnit);
        if (attacked)
            Debug.Log("Exterminate: 적 유닛 공격을 수행했습니다.");
    }
}
