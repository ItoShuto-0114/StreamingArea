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

    void Start()
    {
        Destroy(gameObject, ProjectileLifeTime);
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
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _direction = (player.transform.position - transform.position).normalized;
        }
        _isMoving = true;
    }
    public void Init(Vector3 muzzlePos)
    {
        _muzzleTargetPos = muzzlePos;
        StartCoroutine(ShootBullet());
    }
    void Update()
    {
        if (_isMoving)
        {
            transform.position += _direction * ProjectileSpeed * Time.deltaTime;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Destroy(gameObject);
            Debug.Log("プレイヤーに当たった");
            //ダメージ処理
        }
    }
}
