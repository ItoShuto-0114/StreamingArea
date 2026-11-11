using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    Transform _player;
    List<EnemyBase> _enemyList = new();
    public bool IsStopped { get; private set; }

    public Transform PlayerTransform
    {
        get
        {
            if (_player == null)
                _player = FindFirstObjectByType<TestPlayerMove>()?.transform;//playerにする
            return _player;
        }
    }
    public void SetStop(bool stop)
    {
        IsStopped = stop;
    }
    public void RegisterEnemy(EnemyBase enemy)
    {
        if(!_enemyList.Contains(enemy))
        {
            _enemyList.Add(enemy);
        }
    }
    public void UnRegisterEnemy(EnemyBase enemy)
    {
        _enemyList.Remove(enemy);
    }
    public void ResetState()
    {
        _enemyList.Clear();
        IsStopped = false;
        _player = null;
    }
    public EnemyBase NearistEnemy(List<EnemyBase> list, Vector3 from)
    {
        EnemyBase nearistenemy = null;
        float best = float.MaxValue;
        foreach (EnemyBase nearenemies in list)
        {
            if (nearenemies == null)continue;
            float playertoenemydis = (from - nearenemies.transform.position).sqrMagnitude;//一番近い敵を探す
            if(best > playertoenemydis)
            {
                best = playertoenemydis;
                nearistenemy = nearenemies;
            }
        }
        return nearistenemy;
    }
    public bool IsOnScreen(EnemyBase enemy)//カメラ内かどうか
    {
        Vector3 v = Camera.main.WorldToViewportPoint(enemy.transform.position);
        return v.z > 0 && v.x >= 0 && v.x <= 1 && v.y >= 0 && v.y <= 1;
    }
    public List<EnemyBase> EnemiesOnScreen()//エネミーが画面内か、共振斬り用
    {
        var LockedOnenemys = new List<EnemyBase>();
        foreach (var enemy in _enemyList)
        {
            if (enemy == null) continue;
            if (!IsOnScreen(enemy)) continue;

            LockedOnenemys.Add(enemy);
        }
        return LockedOnenemys;
    }
    public List<EnemyBase> LockedOnEnemy()//エネミーが画面内かつロックオン状態か、音速斬り用
    {
        var result = new List<EnemyBase>();
        foreach (var enemy in EnemiesOnScreen())
        {
            if (enemy.IsLockedOn) result.Add(enemy);
        }
        return result;
    } 
    public void EnemyOnDead(EnemyBase enemy)
    {
        _enemyList.Remove(enemy);
        Debug.Log("残り"+_enemyList.Count);
        if(_enemyList.Count == 0)
        {
            GameManager.Instance?.EndGame(GameResult.Clear);
        }
    }
}
