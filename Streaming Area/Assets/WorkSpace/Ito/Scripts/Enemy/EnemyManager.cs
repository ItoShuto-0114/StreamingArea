using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    List<EnemyBase> _enemyList = new();
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
}
