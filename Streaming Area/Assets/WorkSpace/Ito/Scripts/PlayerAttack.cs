using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("UŒ‚—Í"), SerializeField] float _power;
    [Header("ƒRƒ“ƒ{‚ÌƒŠƒZƒbƒgŽžŠÔ"), SerializeField] float _resetComboTime = 2;
    [Header("Å‘å—­‚ßŽžŠÔ"), SerializeField] float _maxChargeTime = 3;
    int _combo;
    float _timer;
    float _chargeTime;
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
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _chargeTime = 0;
        }
        if (Input.GetKey(KeyCode.Mouse0))
        {
            _chargeTime += Time.deltaTime;
            if ( _chargeTime > _maxChargeTime)
            {
                _chargeTime = _maxChargeTime;
            }
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (_chargeTime <= 0.5)
            {
                Debug.Log("ƒRƒ“ƒ{");
                Attack();
            }
            else
            {
                ChargeAttack();
            }
            _chargeTime = 0;
        }
    }
    void ChargeAttack()
    {
        Debug.Log(_chargeTime * _power + "ƒ_ƒ[ƒW");
    }
    void Attack()
    {
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
