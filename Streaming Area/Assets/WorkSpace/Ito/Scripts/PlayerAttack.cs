using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("UŒ‚—Í"), SerializeField] float _power;
    [Header("ƒRƒ“ƒ{‚ÌƒŠƒZƒbƒgŽžŠÔ"), SerializeField] float _resetComboTime = 2;
    [Header("Å‘å—­‚ßŽžŠÔ"), SerializeField] float _maxChargeTime = 3;
    [Header("’ÊíUŒ‚‚Æƒ`ƒƒ[ƒWUŒ‚‚ðØ‚è‘Ö‚¦‚éŽžŠÔ"),SerializeField] float _chargeRequiredTime = 0.5f;
    int _combo;
    float _timer;
    float _chargeTime;
    bool _isCharging;
    void Update()
    {
        if (_combo != 0)
        {
            _timer += Time.deltaTime;
            if (_timer >= _resetComboTime)
            {
                _combo = 0;
                _timer = 0;
            }
        }
        if(_isCharging)
        {
            _chargeTime += Time.deltaTime;
            if (_chargeTime > _maxChargeTime)
            {
                _chargeTime = _maxChargeTime;
            }
        }
    }
       
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _chargeTime = 0;
            _isCharging = true;
        }
        if (context.canceled)
        {
            if (_chargeTime <= _chargeRequiredTime)
            {
                Attack();
            }
            else
            {
                ChargeAttack();
            }
            _isCharging= false;
            _chargeTime = 0;
        }
    }
    void ChargeAttack()
    {
        _combo = 0;
        _timer = 0;
        Debug.Log(_chargeTime * _power + "ƒ_ƒ[ƒW");
    }
    void Attack()
    {
        _timer = 0;
        switch (_combo)
        {
            case 0:
                Attack1();
                _combo = 1;
                break;

            case 1:
                Attack2();
                _combo = 2;
                break;

            case 2:
                Attack3();
                _combo = 0;
                break;
        }
    }
    void Attack1()
    {
        Debug.Log("UŒ‚‚P");
    }
    void Attack2()
    {
        Debug.Log("UŒ‚‚Q");
    }
    void Attack3()
    {
        Debug.Log("UŒ‚‚R");
    }
}
