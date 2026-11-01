using UnityEngine;
using UnityEngine.InputSystem;

public enum StyleType
{
    None,
    Speed,
    Blade,
    Berserk
}
public class PlayerStyle : MonoBehaviour
{
    public StyleType CurrentStyle { get; private set; } = StyleType.None;
    void ChangeStyle()
    {
         switch(CurrentStyle)
        {
            case StyleType.None:
                CurrentStyle = StyleType.Speed;
                break;

            case StyleType.Speed:
                CurrentStyle = StyleType.Blade;
                break;

            case StyleType.Blade:
                CurrentStyle = StyleType.Berserk;
                break;

            case StyleType.Berserk:
                CurrentStyle = StyleType.Speed;
                break;
        }
    }
    public void OnChangeStyle(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            ChangeStyle();
        }
    }
}