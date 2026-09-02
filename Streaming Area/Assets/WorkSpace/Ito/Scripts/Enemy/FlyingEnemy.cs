using System.Collections;
using UnityEngine;

public class FlyingEnemy : EnemyBase
{
    [Header("認識距離"), SerializeField] float DetectRange = 5;
    [Header("地面からの高さ"), SerializeField] float HoverHeight = 2.5f;
    [Header("ホバリングの揺れの大きさ"), SerializeField] float HoverAmplitude = 0.2f;
    [Header("ホバリングの揺れの速さ"), SerializeField] float HoverFrequency = 2f;
    float _hoverOffSet;
    [Header("ランダム移動の最大移動距離"), SerializeField] float WanderMoveDistance = 1;
    [Header("ランダム移動にかかる時間"), SerializeField] float WanderMoveTime = 0.8f;
    //[Header("移動制限"), ] bool MoveAreaClamp = true;
    //[Header("1セットで打つエナジー弾の数"), SerializeField] int ShotCount = 3; エナジー弾はマズルの数だけ撃つ
    [Header("弾と弾の発射感覚"), SerializeField] float ShotInterval = 0.2f;
    [Header("射撃後の硬直時間"), SerializeField] float ShotCooldownAfterBurst = 5f;

    [Header("タックル攻撃"), SerializeField] bool UseTackle = false;
    [Header("射撃＋移動を何セット後にタックルするか"), SerializeField] int TackleAfterBurstSet = 2;
    [Header("タックル時の移動速度"), SerializeField] float TackleSpeed;
    [Header("タックルの最大継続時間"), SerializeField] float TackleMaxTime;
    [Header("壁・地面に当たる前に止まる距離"), SerializeField] float TackleStopDistance = 1f;
    [Header("タックル後に戻る高さ"), SerializeField] float ReturnHeight = 2.5f;
    [Header("エネルギー弾のPrefab"), SerializeField] GameObject _bullet;
    [Header("エネルギー弾が止まる位置"), SerializeField] Transform[] _stopPos;
    [Header("エネルギー弾が出る位置"), SerializeField] Transform _muzzle;
    Color _warningColor = Color.red;
    Color _originalColor;
    Renderer _rend;
    float _distance;//プレイヤーと敵の距離
    Transform _player;
    [SerializeField]LayerMask _groundLayer;
    Vector3 _centerPos;
    Vector3 _targetX;
    Vector3 _tackleTargetPos;
    Vector3 _tackleDirection;
    bool _isAttack;
    bool _canLockOn;
    bool _isTackle;//タックル中しているかのbool
    bool _isReturning;//タックル後元の高さに戻すためのbool
    bool _isFacingRight;//どこ向いているかのbool
    int _count;
    [Header("点滅回数"),SerializeField] int _blinkcount = 3;
    [Header("点滅時間"), SerializeField] float _blinkinterval = 0.5f;
    float _timer;
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _centerPos = transform.position;
        _targetX = RandomDirection();
        _rend = GetComponent<Renderer>();
        _originalColor = _rend.material.color;
    }


    void Update()
    {
        _hoverOffSet = Mathf.Sin(Time.time * HoverFrequency) * HoverAmplitude;
      #region
        _distance = Vector3.Distance(transform.position , _player.transform.position);//プレイヤーと敵の距離
        if (_distance > DetectRange && !_isAttack)//プレイヤーが範囲外に居たら
        {
            EnemyMove(); //ランダム移動
            _canLockOn = false;
            _count = 0;
        }
        else//プレイヤーが範囲内だったら
        {
            _canLockOn = true;
            Attack();
        }
        #endregion

        if (!_isTackle && !_isReturning)
        {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 10f, _groundLayer))
            {
                transform.position = new Vector3(transform.position.x, hit.point.y + HoverHeight + _hoverOffSet, transform.position.z);
            }
        }
       
    }
    //移動処理
    #region
    void EnemyMove()
    {
        _isFacingRight = _targetX.x > transform.position.x;
        UpdateFacing();
        var target = new Vector3(_targetX.x, transform.position.y, transform.position.z);
        float speed = WanderMoveDistance / WanderMoveTime;
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        if (Mathf.Abs(transform.position.x - _targetX.x) < 0.01f)
        {
            _targetX = RandomDirection();
        }
    }
    
    private Vector3 RandomDirection()//ランダムな方向を決めるメソッド
    {
        float x = Random.Range(-WanderMoveDistance, WanderMoveDistance);

        return _centerPos + new Vector3(x,0f,0f);
    }
    #endregion
    public void UpdateFacing()
    {
        if(_isFacingRight)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
    //攻撃処理
    #region
    protected override void Attack()
    {
        if (_isAttack)
        {
            if (_player.position.x < transform.position.x)
            {
                _isFacingRight = false;
            }
            else
            {
                _isFacingRight = true;
            }
            UpdateFacing();
        }
        if (_isAttack) return;
        if (UseTackle && _count >= TackleAfterBurstSet)
        {
            StartCoroutine(TackleRoutine());
        }
        else
        {
            StartCoroutine(ShootRoutine());
        }
    }
    
    //タックル処理
    public IEnumerator TackleRoutine()
    {
        _isAttack = true;
        yield return StartCoroutine(Blink());
        _tackleTargetPos = _player.transform.position;
        _tackleDirection = (_tackleTargetPos - transform.position).normalized;
        _tackleDirection.z = 0f;
        _isTackle= true;
        _timer = 0;
       
            while (TackleMaxTime>_timer)
        {
            if (Physics.Raycast(transform.position, _tackleDirection, TackleStopDistance, _groundLayer))
            {
                break;
            }
            transform.position += _tackleDirection * TackleSpeed * Time.deltaTime;
            _timer += Time.deltaTime;
            yield return null;
        }
        _isTackle = false;
        yield return StartCoroutine(ReturnHeightRoutine());
        _count = 0;
        _isAttack = false;
    }

    public IEnumerator ReturnHeightRoutine()//元の高さに戻すコルーチン
    {
        _isReturning = true;
        while (true)
        {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 10f, _groundLayer))
            {
                float targetY = hit.point.y + ReturnHeight;
                float newY = Mathf.MoveTowards(transform.position.y, targetY, TackleSpeed * Time.deltaTime);
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);
                if(Mathf.Abs(newY - targetY) < 0.01f)
                {
                    break;
                }
            }
            else
            {
                break;
            }
            yield return null;
        }
        _isReturning = false;
    }

    //遠距離攻撃処理
    public IEnumerator ShootRoutine()
    {
        _isAttack = true;
        foreach (Transform muzzle in _stopPos)
        {
            Shoot(muzzle);
            yield return new WaitForSeconds(ShotInterval);
        }
        yield return new WaitForSeconds(ShotCooldownAfterBurst);
        _count++;
        _isAttack = false;
    }
    void Shoot(Transform muzzle)
    {
        GameObject bulletObj = Instantiate(_bullet, _muzzle.position, Quaternion.identity);
        if (bulletObj.TryGetComponent<EnemyBullet>(out var bullet))
        {
            bullet.Init(muzzle.position); // 本来の目標位置を渡す
        }
    }
    #endregion
    public IEnumerator Blink()//タックル直前点滅するメソッド
    {
        for (int i = 0; i < _blinkcount; i++)
        {
            _rend.material.color = _warningColor;
            yield return new WaitForSeconds(_blinkinterval);
            _rend.material.color = _originalColor;
            yield return new WaitForSeconds(_blinkinterval);
        }
    }

    protected override void Dead()
    {
        Destroy(gameObject);
    }
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
    }
   
}
