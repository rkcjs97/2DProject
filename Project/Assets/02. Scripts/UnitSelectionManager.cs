using UnityEngine;

public class UnitSelectionManager : MonoBehaviour
{
    [SerializeField] private CameraTargetController cameraTargetController;
    
    [SerializeField] private Unit selectedUnit;
    public Unit SelectedUnit => selectedUnit;
    private void Awake()
    {
        if(cameraTargetController == null)
            cameraTargetController = FindObjectOfType<CameraTargetController>();
    }
    
    public void SelectUnit(Unit unit)
    {
        if (unit == null)
            return;
        
        if(selectedUnit != null)
            selectedUnit.SetSelected(false);
        
        selectedUnit = unit; 
        selectedUnit.SetSelected(true);
        
        if (cameraTargetController != null)
            cameraTargetController.MoveToTarget(unit.transform);
        
        Debug.Log($"선택된 유닛: {unit.unitName}");

    }

    public void DeselectUnit()
    {
        if (selectedUnit != null)
            selectedUnit.SetSelected(false);
        
        selectedUnit = null;
        Debug.Log("유닛 선택 해제");   
        
        /*if(cameraTargetController != null) 
            cameraTargetController.ClearTarget();*/
    }

}
