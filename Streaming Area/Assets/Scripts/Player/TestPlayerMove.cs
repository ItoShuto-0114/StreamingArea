using UnityEngine;
using UnityEngine.InputSystem;

public class TestPlayerMove : MonoBehaviour
{
    [SerializeField] float _playerSpeed;
    [SerializeField] float _jumpForce = 5;
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
    public void OnJump(InputAction.CallbackContext con)
    {
        if (con.performed)
        {
            Jump();
        }
    }
    void FixedUpdate()
    {
        _rb.linearVelocity = new Vector3(_move.x * _playerSpeed, _rb.linearVelocity.y); 
    }
    void Jump()
    {
        _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
    }
}
