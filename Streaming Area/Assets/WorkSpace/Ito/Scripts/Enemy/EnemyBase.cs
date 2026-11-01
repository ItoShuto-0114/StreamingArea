using UnityEngine;

public class EnemyBase : MonoBehaviour,IDamageable
{
    [Header("HP")]
    [SerializeField] protected float _hp;
    [Header("スピード")]
    [SerializeField] protected float _speed;
    [Header("攻撃力")]
    [SerializeField] protected float _power;
    public virtual void TakeDamage(float damage)
    {
        _hp -= damage;
        if(_hp<=0)
        {
            Dead();
        }
    }
    protected virtual void Dead()
    {
        
    }
    protected virtual void Attack()
    {
        
    }
    
}
