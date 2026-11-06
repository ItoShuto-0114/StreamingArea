using System.Collections;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [Header("エナジー弾の速度"), SerializeField] float ProjectileSpeed = 10;
    [Header("とどまる時間"), SerializeField] float _delayTime = 3;
    [Header("エネルギー弾のダメージ"), SerializeField] int ProjectileDamage;
    [Header("エネルギー弾が消えるまでの時間"), SerializeField] float ProjectileLifeTime = 5;
    [Header("発射口から止まる位置までかかる時間"), SerializeField] float _moveToMuzzleTime = 0.2f; 
    Vector3 _direction;
    bool _isMoving = false;
    Vector3 _muzzleTargetPos;
    ObjectPool<EnemyBullet> _bulletPool;
    void Update()
    {
        if (_isMoving)
        {
            transform.position += _direction * ProjectileSpeed * Time.deltaTime;
        }
    }
    IEnumerator LifeRoutine()
    {
        yield return new WaitForSeconds(ProjectileLifeTime);
        Release();
    }
    IEnumerator ShootBullet()
    {
        Vector3 startPos = transform.position;
        float timer = 0f;

        // 発射口からマズル位置まで、なめらかに移動
        while (timer < _moveToMuzzleTime)
        {
            transform.position = Vector3.Lerp(startPos, _muzzleTargetPos, timer / _moveToMuzzleTime);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.position = _muzzleTargetPos; // 誤差対策
        yield return new WaitForSeconds(_delayTime);
        var player = EnemyManager.Instance?.PlayerTransform;
        if (player != null)
        {
            _direction = (player.position - transform.position).normalized;
        }
        else
        {
            _direction = Vector3.zero;
        }
        _isMoving = true;
    }
    public void Init(Vector3 muzzlePos,ObjectPool<EnemyBullet> pool)
    {
        _bulletPool = pool;
        _muzzleTargetPos = muzzlePos;
        _isMoving = false;
       StartCoroutine(LifeRoutine());
        StartCoroutine(ShootBullet());
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Release();
            Debug.Log("プレイヤーに当たった");
            //ダメージ処理
        }
    }
    void Release()
    {
        if (_bulletPool != null) _bulletPool.ReturnObject(this);
        else
        {
            Destroy(gameObject);
        }
    }
}
