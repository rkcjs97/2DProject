using Unity.Cinemachine;
using UnityEngine;

public class CameraTargetController : MonoBehaviour
{
    /*[SerializeField] private CinemachineCamera cinemachineCamera;*/
    [SerializeField] private Transform CameraTarget;

    public void MoveToTarget(Transform target)
    {
        if(CameraTarget == null || target == null)
            return;
        
        CameraTarget.position = target.position;
    }
    /*private void Awake()
   {
        if (cinemachineCamera == null)
             cinemachineCamera = FindObjectOfType<CinemachineCamera>();
   }
    
     public void SetTarget(Transform target)
     {
         if (cinemachineCamera == null || target == null)
             return;
         
       cinemachineCamera.Target.TrackingTarget = target;
     }
    
     public void ClearTarget()
     {
        if (cinemachineCamera == null)
            return;
         
        cinemachineCamera.Target.TrackingTarget = null;
     }*/
}
