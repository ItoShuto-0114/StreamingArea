using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct HitBox
{
    [Tooltip("ìñÇΩÇËîªíËÇÃñºëO")] public string Name;
    [Tooltip("ÉvÉåÉCÉÑÅ[Ç©ÇÁÇÃãóó£")] public Vector3 Offset;
    [Tooltip("ìñÇΩÇËîªíËÇÃÉTÉCÉY")]public Vector3 Size;
    [Tooltip("çUåÇóÕÇ…ä|ÇØÇÈî{ó¶")]public float DamageRate;
    [Tooltip("çUåÇÇó£ÇµÇƒÇ©ÇÁîªíËÇ™èoÇÈÇ‹Ç≈")]public float Startup;
    [Tooltip("îªíËÇ™èoÇƒÇ¢ÇÈéûä‘")]public float Active;
    [Tooltip("îªíËÇ™è¡Ç¶ÇƒÇ©ÇÁéüÇÃì¸óÕÇéÛÇØÇÈÇ‹Ç≈")]public float Recovery;
    [Tooltip("ìñÇΩÇ¡ÇΩéûÇ…í«â¡Ç≥ÇÍÇÈîMã∂íl")] public float FeverPoint;
}
[System.Serializable]
public class PlayerAttack
{
    
    [Header("çUåÇóÕ"), SerializeField] float _power;

    [Header("âπë¨éaÇË")]
    [Header("É_ÉÅÅ[ÉWî{ó¶"), SerializeField] float _sonicSlashpowerRate = 1.25f;
    [Header("à⁄ìÆéûä‘"), SerializeField] float _dashTime = 0.1f;
    [Header("éaÇÈä‘äu"), SerializeField] float _sonicSlashInterval;
    [Header("ìGÇÃãóó£"), SerializeField] Vector3 _offset = new Vector3(3, 0, 0);
    [Header("îMã∂ílÇÃíl"), SerializeField] float _sonicSlashpoint = 500;

    [Header("ã§êUéaÇË")]
    [Header("É_ÉÅÅ[ÉWî{ó¶"), SerializeField] float _resonantSlashpowerRate =1.5f;
    [Header("éûä‘Çé~ÇﬂÇÈïbêî"), SerializeField] float _resonantTime = 3f;

    [Header("É`ÉÉÅ[ÉWéaÇË")][Header("ÉRÉìÉ{ÇÃÉäÉZÉbÉgéûä‘"), SerializeField] float _resetComboTime = 2;
    [Header("ç≈ëÂó≠Çﬂéûä‘"), SerializeField] float _maxChargeTime = 3;
    [Header("í èÌçUåÇÇ∆É`ÉÉÅ[ÉWçUåÇÇêÿÇËë÷Ç¶ÇÈéûä‘"), SerializeField] float _chargeRequiredTime = 0.5f;

    [Header("ìñÇΩÇËîªíË")]
    [SerializeField] HitBox _firstAttack = new() { Name = "àÍíiñ⁄", Offset = new Vector3(1, 0, 0), Size = new Vector3(1, 1, 2), DamageRate = 1, Startup = 0.1f, Active = 0.1f, Recovery = 0.1f, FeverPoint = 50f };
    [SerializeField] HitBox _secondAttack = new() { Name = "ìÒíiñ⁄", Offset = new Vector3(1, 0, 0), Size = new Vector3(1, 1, 2), DamageRate = 1, Startup = 0.1f, Active = 0.1f, Recovery = 0.1f, FeverPoint = 70f };
    [SerializeField] HitBox _thirdAttack = new() { Name = "éOíiñ⁄", Offset = new Vector3(1, 0, 0), Size = new Vector3(1.5f, 1, 2), DamageRate = 1, Startup = 0.1f, Active = 0.1f, Recovery = 0.1f, FeverPoint = 100f };
    [SerializeField] HitBox _chargeAttack = new() { Name = "É`ÉÉÅ[ÉWçUåÇ", Offset = new Vector3(1, 0, 0), Size = new Vector3(2, 1, 2), DamageRate = 1, Startup = 0.1f, Active = 0.1f, Recovery = 0.1f, FeverPoint = 200f };
    [SerializeField] HitBox _jumpAttack = new() { Name = "ÉWÉÉÉìÉvçUåÇ", Offset = new Vector3(1, 0, 0), Size = new Vector3(2, 1, 2), DamageRate = 1, Startup = 0.1f, Active = 0.1f, Recovery = 0.1f, FeverPoint = 150f };
    [SerializeField] HitBox _dashAttack = new() { Name = "É_ÉbÉVÉÖçUåÇ", Offset = new Vector3(1, 0, 0), Size = new Vector3(2, 1, 2), DamageRate = 1, Startup = 0.1f, Active = 0.1f, Recovery = 0.1f, FeverPoint = 200f };
    Transform _player;
    int _facing;
    int _combo;
    float _timer;
    float _chargeTime;
    bool _isCharging;
    bool _isAttacking;
    HitBox _activeBox;
    bool _showBox;
    float _attackSpeedRate = 1;
    float _powerRate = 1;
    float _hitBoxRate = 1;
    
    ComboCounter _counter;
    public void TakePlayerInfo(Transform player, int facing)
    {
        _player = player;
        _facing = facing;
    }
    public void UpdateAttack()
    {
        ComboTimerCheck();
    }
    public void ComboTimerCheck()
    {
        _timer += Time.deltaTime;
        if (_timer >= _resetComboTime)
        {
            _combo = 0;
            _timer = 0;
        }
    }
    public IEnumerator ChargeRelease(bool isDashing, bool isJumping)
    {
        float charge = _chargeTime;
        _chargeTime = 0;
        _isCharging = false;
        if (_isAttacking) yield break;

        if (isJumping) yield return JumpAttack();
        else if (isDashing) yield return DashAttack();
        else if (charge <= _chargeRequiredTime) yield return Attack();
        else yield return ChargeAttack(charge);
    }
    public void Charging()
    {
        _isCharging = true;
        _chargeTime += Time.deltaTime;
        if (_chargeTime > _maxChargeTime)
        {
            _chargeTime = _maxChargeTime;
        }
    }
    IEnumerator ChargeAttack(float charge)
    {
        _combo = 0;
        _timer = 0;
        yield return AttackHitRoutine(_chargeAttack,_chargeAttack.Name ,charge * _power * _chargeAttack.DamageRate * _powerRate);
    }
    IEnumerator Attack()
    {
        _timer = 0;
        switch (_combo)
        {
            case 0:
                _combo = 1;
                yield return AttackHitRoutine(_firstAttack,_firstAttack.Name, _power * _firstAttack.DamageRate * _powerRate);
                break;
            case 1:
                _combo = 2;
                yield return AttackHitRoutine(_secondAttack,_secondAttack.Name, _power * _secondAttack.DamageRate * _powerRate);
                break;
            case 2:
                _combo = 0;
                yield return AttackHitRoutine(_thirdAttack,_thirdAttack.Name, _power * _thirdAttack.DamageRate * _powerRate);
                break;
        }
    }
    public IEnumerator DashAttack()
    {
        yield return AttackHitRoutine(_dashAttack,_dashAttack.Name,_power * _dashAttack.DamageRate  * _powerRate);
    }
    public IEnumerator JumpAttack()
    {
        yield return AttackHitRoutine(_jumpAttack,_jumpAttack.Name, _power * _jumpAttack.DamageRate * _powerRate);
    }
    public IEnumerator SonicSlash()//âπë¨éaÇË
    {
        var LockOnEnemies = EnemyManager.Instance?.LockedOnEnemy();
        if (_player == null)
        {
            _player = EnemyManager.Instance?.PlayerTransform;
        }
        if (_player == null || LockOnEnemies == null) yield break;
        Vector3 origin = _player.transform.position;
            while (LockOnEnemies.Count > 0)
            {
                var NextEnemy = EnemyManager.Instance?.NearistEnemy(LockOnEnemies, origin);
                if (NextEnemy == null)
                {
                    yield break;
                }
                LockOnEnemies.Remove(NextEnemy);
                origin = NextEnemy.transform.position + _offset;
                yield return SonicDash(origin);
                if (NextEnemy != null)
                {
                   NextEnemy.TakeDamage(_power * _sonicSlashpowerRate * _powerRate);
                  _counter?.AddFever(_sonicSlashpoint, "âπë¨éaÇËÇ™ìñÇΩÇ¡ÇΩÅI");
                }
           
                
                yield return new WaitForSeconds(_sonicSlashInterval);
            }
    }
    public IEnumerator SonicDash(Vector3 target)
    {
        Vector3 Start = _player.transform.position;
        float t = 0;
        while (t < _dashTime)
        {
            t += Time.deltaTime;
            _player.position = Vector3.Lerp(Start, target, t / _dashTime);
            yield return null;
        }
        _player.position = target;
    }
    public IEnumerator ResonantSlash()//ã§êUéaÇË
    {
        EnemyManager.Instance?.SetStop(true);
        var ScreenOnEnemy = EnemyManager.Instance?.EnemiesOnScreen();
        if (ScreenOnEnemy == null)
        {
            EnemyManager.Instance?.SetStop(false); yield break;
        }
        foreach(var e in ScreenOnEnemy)
        {
            if (e == null) continue;
            e.TakeDamage(_power * _resonantSlashpowerRate * _powerRate);
        }
        yield return new WaitForSeconds(_resonantTime);
        EnemyManager.Instance?.SetStop(false);
    }
    
    IEnumerator AttackHitRoutine(HitBox box,string name, float damage)
    {
        _isAttacking = true;
        Debug.Log(name);
        yield return new WaitForSeconds(box.Startup/_attackSpeedRate);

        _activeBox = box;
        _showBox = true;
        var hitcol = new HashSet<Collider>();
        float timer = 0;
        while (timer < box.Active/_attackSpeedRate)
        {
            DoHit(_player.position, _facing, box, damage, hitcol);
            timer += Time.deltaTime;
            yield return null;
        }
        _showBox = false;
        yield return new WaitForSeconds(box.Recovery/_attackSpeedRate);
        _isAttacking = false;
    }
    public void DoHit(Vector3 origin, int facing, HitBox box, float damage, HashSet<Collider> hitcol)
    {
        Vector3 center = origin + new Vector3(box.Offset.x * facing, box.Offset.y, 0f);
        Collider[] hits = Physics.OverlapBox(center, box.Size / 2 * _hitBoxRate, Quaternion.identity);
        foreach (var c in hits)
        {
            if (c.transform.root == _player.root) continue;
            if (!hitcol.Add(c)) continue;//Ç∑Ç≈Ç…ìñÇΩÇ¡ÇƒÇ¢ÇΩÇÁîÚÇŒÇ∑
            if (c.TryGetComponent<IDamageable>(out var target))
            {
                target.TakeDamage(damage);
                _counter?.AddFever(box.FeverPoint, box.Name + "Ç™ìñÇΩÇ¡ÇΩ");
                Debug.Log(target + "Ç…" + damage + "É_ÉÅÅ[ÉW!");
            }
        }
    }
    public void OnDrawGizmos(Vector3 origin)
    {
        if (!_showBox) return;
        Vector3 center = origin + new Vector3(_activeBox.Offset.x * _facing, _activeBox.Offset.y, 0f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(center, _activeBox.Size * _hitBoxRate);
    }
    public void SetFever(ComboCounter Counter)
    {
        _counter = Counter;
    }
    public void SetRate(float powerrate, float attackspeedrate,float hitboxrate)
    {
        _powerRate = powerrate;
        _attackSpeedRate = attackspeedrate;
        _hitBoxRate = hitboxrate;
    }
}
