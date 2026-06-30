using UnityEngine;
using UnityEngine.InputSystem;

public class TestPlayerMove : MonoBehaviour
{
    [SerializeField] float _playerSpeed;
    Rigidbody _rb;
    Vector2 _move;
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _move = context.ReadValue<Vector2>();
    }
    
    void FixedUpdate()
    {
        _rb.linearVelocity = new Vector3(_move.x * _playerSpeed, _rb.linearVelocity.y); 
    }
}
