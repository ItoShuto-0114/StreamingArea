using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public struct HitBox
{
    public Vector3 Offset;//プレイヤーからどれだけ離すか
    public Vector3 Size;//ボックスのサイズ
    public float DamageRate;//_power に掛ける倍率
    public float Startup;//攻撃ボタンを離してから判定が出るまで
    public float Active;//判定が出ている時間
    public float Recovery;//判定が消えてから次の入力を受けるまで
}
[System.Serializable]
public class PlayerAttack
{
    [Header("攻撃力"), SerializeField] float _power;
    [Header("コンボのリセット時間"), SerializeField] float _resetComboTime = 2;
    [Header("最大溜め時間"), SerializeField] float _maxChargeTime = 3;
    [Header("通常攻撃とチャージ攻撃を切り替える時間"),SerializeField] float _chargeRequiredTime = 0.5f;
    [Header("当たり判定")]
    [SerializeField] HitBox _firstAttack = new() { Offset = new Vector3(1,0,0), Size = new Vector3(1,1,2) ,DamageRate = 1,Startup = 1,Active = 0.5f,Recovery = 0.5f};
    [SerializeField] HitBox _secondAttack = new() { Offset = new Vector3(1,0,0), Size = new Vector3(1,1,2), DamageRate = 1, Startup = 1, Active = 0.5f, Recovery = 0.5f };
    [SerializeField] HitBox _thirdAttack = new() { Offset = new Vector3(1,0,0), Size = new Vector3(1.5f,1,2), DamageRate = 1, Startup = 1, Active = 0.5f, Recovery = 0.5f };
    [SerializeField] HitBox _chargeAttack = new() { Offset = new Vector3(1,0,0), Size = new Vector3(2,1,2), DamageRate = 1, Startup = 1, Active = 0.5f, Recovery = 0.5f };
    Transform _player;
    int _facing;
    int _combo;
    float _timer;
    float _chargeTime;
    bool _isCharging;
    bool _isAttacking;
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
    public IEnumerator ChargeRelease()//通常三段かチャージか判定
    {
        float charge = _chargeTime;
        _chargeTime = 0;
        _isCharging = false;
        if (_isAttacking) yield break;
        if (charge <= _chargeRequiredTime) yield return Attack();
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
        yield return AttackHitRoutine(_chargeAttack, charge * _power * _chargeAttack.DamageRate);
    }
    public void DoHit(Vector3 origin,int facing,HitBox box,float damage,HashSet<Collider> hitcol)
    {
        Vector3 center = origin + new Vector3(box.Offset.x * facing,box.Offset.y,0f);
        Collider[] hits = Physics.OverlapBox(center, box.Size/2,Quaternion.identity);
        foreach (var c in hits)
        {
            if (c.transform.root == _player.root) continue;
            if (!hitcol.Add(c)) continue;//すでに当たっていたら飛ばす
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
                yield return AttackHitRoutine(_firstAttack, _power * _firstAttack.DamageRate);
                break;
            case 1:
                _combo = 2;
                yield return AttackHitRoutine(_secondAttack, _power * _secondAttack.DamageRate);
                break;
            case 2:
                _combo = 0;
                yield return AttackHitRoutine(_thirdAttack, _power * _thirdAttack.DamageRate);
                break;
        }
    }
    public void ResonantSlash()
    {

    }
    public void SonicSlash()
    {

    }
    IEnumerator AttackHitRoutine(HitBox box, float damage)
    {
        _isAttacking = true;
        yield return new WaitForSeconds(box.Startup);

        var hitcol = new HashSet<Collider>();
        float timer = 0;
        while (timer < box.Active)
        {
            DoHit(_player.position, _facing, box, damage, hitcol);
            timer += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(box.Recovery);
        _isAttacking = false;
    }
}
