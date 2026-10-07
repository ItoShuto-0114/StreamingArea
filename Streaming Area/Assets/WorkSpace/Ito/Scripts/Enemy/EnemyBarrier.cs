using UnityEngine;

public class EnemyBarrier : MonoBehaviour
{
    [Header("防壁生成時にプレイヤーを押し出すか"), SerializeField] bool BarrierPushPlayer = true;
    [Header("防壁に接触したときのダメージ"), SerializeField] int BarrierDamageOnTouch;
    [Header("プレイヤーの攻撃何発で防壁が壊れるか"), SerializeField] int BarrierHP;
    [Header("防壁が自動消滅するまでの時間"), SerializeField] float BarrierLifeTime;
    [Header("バリアの最大の傾き"), SerializeField] float MaxBarrierAngle = 30;
    [Header("バリアのスピード"),SerializeField] float _barrierSpeed = 10;
    Vector3 _moveDir;
    Collider _col;
    void Start()
    {
        _col = GetComponent<Collider>();
       if(BarrierPushPlayer)
        {
            _col.isTrigger = false;
        }
       else
        {
            _col.isTrigger = true;
        }
        float angle = Random.Range(-MaxBarrierAngle, MaxBarrierAngle);
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        Destroy(gameObject, BarrierLifeTime);
    }

    void Update()
    {
        transform.position += _moveDir * _barrierSpeed * Time.deltaTime;
    }
    public void BarrierInit(Vector3 moveDir)
    {
        _moveDir = moveDir;
    }

    private void OnCollisionEnter(Collision col)
    {
        //ダメージ処理
        //if(col.collider.TryGetComponent<>)
    }

}
