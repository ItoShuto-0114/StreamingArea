using System.Collections;
using UnityEngine;

public class EnemyBase : MonoBehaviour, IDamageable
{
    [Header("HP")]
    [SerializeField] protected float _hp;
    [Header("スピード")]
    [SerializeField] protected float _speed;
    [Header("攻撃力")]
    [SerializeField] protected float _power;
    [Header("ノイズ状態の時間")]
    [SerializeField] protected float _duration;
    protected bool _isLockOn = false;
    Coroutine _lockOnCoroutine;
    protected Transform PlayerTransform => EnemyManager.Instance.PlayerTransform;
    public bool IsLockedOn
    {
        get { return _isLockOn; }
    }
    [ContextMenu("TestDamage")]
    void TesrDamage()
    {
        TakeDamage(9999);
    }
    protected virtual void Start()
    {
        EnemyManager.Instance?.RegisterEnemy(this);
    }
    protected virtual void OnDestroy()
    {
        EnemyManager.Instance?.UnRegisterEnemy(this);
    }
    public virtual void LockOn()
    {
        if (_lockOnCoroutine != null)
        {
            StopCoroutine(_lockOnCoroutine);
        }
        _lockOnCoroutine = StartCoroutine(LockOnRoutine());
    }
    protected bool IsStopped()
    {
        if(EnemyManager.Instance != null && EnemyManager.Instance.IsStopped)
        {
            return true;
        }
        else
        {
            return false;
        } 
    }
    IEnumerator LockOnRoutine()
    { 
        _isLockOn = true;
        yield return new WaitForSeconds(_duration);
        _isLockOn = false;
    }
    public virtual void TakeDamage(float damage)
    {
        _hp -= damage;
        if (_hp <= 0)
        {
            Dead();
        }
    }
    protected virtual void Dead() 
    {
        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.EnemyOnDead(this);
        }
        Destroy(gameObject);
    }
    protected virtual void Attack() { }
}
