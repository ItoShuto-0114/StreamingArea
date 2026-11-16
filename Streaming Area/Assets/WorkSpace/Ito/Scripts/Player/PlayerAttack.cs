using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct HitBox
{
    [Tooltip("“–‚½‚è”»’è‚Ì–¼‘O")] public string Name;
    [Tooltip("ƒvƒŒƒCƒ„[‚©‚ç‚Ì‹——£")] public Vector3 Offset;
    [Tooltip("“–‚½‚è”»’è‚ÌƒTƒCƒY")]public Vector3 Size;
    [Tooltip("UŒ‚—Í‚ÉŠ|‚¯‚é”{—¦")]public float DamageRate;
    [Tooltip("UŒ‚‚ð—£‚µ‚Ä‚©‚ç”»’è‚ªo‚é‚Ü‚Å")]public float Startup;
    [Tooltip("”»’è‚ªo‚Ä‚¢‚éŽžŠÔ")]public float Active;
    [Tooltip("”»’è‚ªÁ‚¦‚Ä‚©‚çŽŸ‚Ì“ü—Í‚ðŽó‚¯‚é‚Ü‚Å")]public float Recovery;
}
[System.Serializable]
public class PlayerAttack
{
    
    [Header("UŒ‚—Í"), SerializeField] float _power;

    [Header("‰¹‘¬Ža‚è")]
    [Header("ƒ_ƒ[ƒW”{—¦"), SerializeField] float _sonicSlashpowerRate = 1.25f;
    [Header("ˆÚ“®ŽžŠÔ"), SerializeField] float _dashTime = 0.1f;
    [Header("Ža‚éŠÔŠu"), SerializeField] float _sonicSlashInterval;
    [Header("“G‚Ì‹——£"), SerializeField] Vector3 _offset = new Vector3(3, 0, 0);

    [Header("‹¤UŽa‚è")]
    [Header("ƒ_ƒ[ƒW”{—¦"), SerializeField] float _resonantSlashpowerRate =1.5f;
    [Header("ŽžŠÔ‚ðŽ~‚ß‚é•b”"), SerializeField] float _resonantTime = 3f;

    [Header("ƒ`ƒƒ[ƒWŽa‚è")][Header("ƒRƒ“ƒ{‚ÌƒŠƒZƒbƒgŽžŠÔ"), SerializeField] float _resetComboTime = 2;
    [Header("Å‘å—­‚ßŽžŠÔ"), SerializeField] float _maxChargeTime = 3;
    [Header("’ÊíUŒ‚‚Æƒ`ƒƒ[ƒWUŒ‚‚ðØ‚è‘Ö‚¦‚éŽžŠÔ"), SerializeField] float _chargeRequiredTime = 0.5f;
  
    [Header("“–‚½‚è”»’è")]
    [SerializeField] HitBox _firstAttack = new() { Name="ˆê’i–Ú",Offset = new Vector3(1, 0, 0), Size = new Vector3(1, 1, 2), DamageRate = 1, Startup = 0.1f, Active = 0.1f, Recovery = 0.1f };
    [SerializeField] HitBox _secondAttack = new() { Name = "“ñ’i–Ú", Offset = new Vector3(1, 0, 0), Size = new Vector3(1, 1, 2), DamageRate = 1, Startup = 0.1f, Active = 0.1f, Recovery = 0.1f };
    [SerializeField] HitBox _thirdAttack = new() { Name = "ŽO’i–Ú", Offset = new Vector3(1, 0, 0), Size = new Vector3(1.5f, 1, 2), DamageRate = 1, Startup = 0.1f, Active = 0.1f, Recovery = 0.1f };
    [SerializeField] HitBox _chargeAttack = new() { Name = "ƒ`ƒƒ[ƒW", Offset = new Vector3(1, 0, 0), Size = new Vector3(2, 1, 2), DamageRate = 1, Startup = 0.1f, Active = 0.1f, Recovery = 0.1f };
    [SerializeField] HitBox _jumpAttack = new() { Name = "ƒWƒƒƒ“ƒvUŒ‚", Offset = new Vector3(1, 0, 0), Size = new Vector3(2, 1, 2), DamageRate = 1, Startup = 0.1f, Active = 0.1f, Recovery = 0.1f };
    [SerializeField] HitBox _dashAttack = new() { Name = "ƒ_ƒbƒVƒ…UŒ‚", Offset = new Vector3(1, 0, 0), Size = new Vector3(2, 1, 2), DamageRate = 1, Startup = 0.1f, Active = 0.1f, Recovery = 0.1f };
    Transform _player;
    int _facing;
    int _combo;
    float _timer;
    float _chargeTime;
    bool _isCharging;
    bool _isAttacking;
    HitBox _activeBox;
    bool _showBox;
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
        yield return AttackHitRoutine(_chargeAttack,_chargeAttack.Name ,charge * _power * _chargeAttack.DamageRate);
    }
    public void DoHit(Vector3 origin,int facing,HitBox box,float damage,HashSet<Collider> hitcol)
    {
        Vector3 center = origin + new Vector3(box.Offset.x * facing,box.Offset.y,0f);
        Collider[] hits = Physics.OverlapBox(center, box.Size/2,Quaternion.identity);
        foreach (var c in hits)
        {
            if (c.transform.root == _player.root) continue;
            if (!hitcol.Add(c)) continue;//‚·‚Å‚É“–‚½‚Á‚Ä‚¢‚½‚ç”ò‚Î‚·
            if (c.TryGetComponent<IDamageable>(out var target))
            {
                target.TakeDamage(damage);
            }
        }

    }

    IEnumerator Attack()
    {
        _timer = 0;
        switch (_combo)
        {
            case 0:
                _combo = 1;
                yield return AttackHitRoutine(_firstAttack,_firstAttack.Name, _power * _firstAttack.DamageRate);
                break;
            case 1:
                _combo = 2;
                yield return AttackHitRoutine(_secondAttack,_secondAttack.Name, _power * _secondAttack.DamageRate);
                break;
            case 2:
                _combo = 0;
                yield return AttackHitRoutine(_thirdAttack,_thirdAttack.Name, _power * _thirdAttack.DamageRate);
                break;
        }
    }
    public IEnumerator DashAttack()
    {
        yield return AttackHitRoutine(_dashAttack,_dashAttack.Name,_power * _dashAttack.DamageRate);
    }
    public IEnumerator JumpAttack()
    {
        yield return AttackHitRoutine(_jumpAttack,_jumpAttack.Name, _power * _jumpAttack.DamageRate);
    }
    public IEnumerator SonicSlash()//‰¹‘¬Ža‚è
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
                yield return Dash(origin);
                if (NextEnemy != null) NextEnemy.TakeDamage(_power * _sonicSlashpowerRate);
                yield return new WaitForSeconds(_sonicSlashInterval);
            }
    }
    public IEnumerator Dash(Vector3 target)
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
    public IEnumerator ResonantSlash()//‹¤UŽa‚è
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
            e.TakeDamage(_power * _resonantSlashpowerRate);
        }
        yield return new WaitForSeconds(_resonantTime);
        EnemyManager.Instance?.SetStop(false);
    }
    
    IEnumerator AttackHitRoutine(HitBox box,string name, float damage)
    {
        _isAttacking = true;
        Debug.Log(name);
        yield return new WaitForSeconds(box.Startup);

        _activeBox = box;
        _showBox = true;
        var hitcol = new HashSet<Collider>();
        float timer = 0;
        while (timer < box.Active)
        {
            DoHit(_player.position, _facing, box, damage, hitcol);
            timer += Time.deltaTime;
            yield return null;
        }
        _showBox = false;
        yield return new WaitForSeconds(box.Recovery);
        _isAttacking = false;
    }
    public void OnDrawGizmos(Vector3 origin)
    {
        if (!_showBox) return;
        Vector3 center = origin + new Vector3(_activeBox.Offset.x * _facing, _activeBox.Offset.y, 0f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(center, _activeBox.Size);
    }
}
