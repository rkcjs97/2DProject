using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMoveController : MonoBehaviour
{
    [Header("이동속도")]
    [SerializeField] private float moveSpeed = 10f;

    [Header("경계 제한 (옵션)")] 
    [SerializeField]
    private Vector2 minBounds;
    [SerializeField]
    private Vector2 maxBounds;

    private void Update()
    {
        Vector2 input = Vector2.zero;

        if (Keyboard.current == null)
            return;

        if (Keyboard.current.wKey.isPressed) input.y += 1;
        if (Keyboard.current.sKey.isPressed) input.y -= 1;
        if (Keyboard.current.aKey.isPressed) input.x -= 1;
        if (Keyboard.current.dKey.isPressed) input.x += 1;

        if (input == Vector2.zero)
            return;
        
        Vector3 moveDir = new Vector3(input.x, input.y, 0f).normalized;
        
        transform.position += moveDir * moveSpeed * Time.deltaTime;

        ClampPositon();
    }

    private void ClampPositon()
    {
        Vector3 pos = transform.position;
        
        pos.x = Mathf.Clamp(pos.x, minBounds.x, maxBounds.x);
        pos.y = Mathf.Clamp(pos.y, minBounds.y, maxBounds.y);
        
        transform.position = pos;
    }
    
    
}
