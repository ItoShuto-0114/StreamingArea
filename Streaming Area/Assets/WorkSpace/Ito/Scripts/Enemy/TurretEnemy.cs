using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretEnemy : EnemyBase,IDamageable
{
    [Header("索敵範囲の横幅"), SerializeField] float DetectWidth;
    [Header("索敵範囲の縦幅"), SerializeField] float DetectHeight;
    [Header("レーザーの長さ"), SerializeField] float LaserLength = 10;
    [Header("レーザーの発射位置"), SerializeField] Transform LaserOrigin;
    [Header("砲台とプレイヤーが同じY座標とみなす許容範囲"), SerializeField] float SameYThreshold = 3;
    [Header("視認してからレーザーを発射するまでの予兆時間"), SerializeField] float LaserStartDelay = 3;
    [Header("レーザーの攻撃判定が残る時間"), SerializeField] float LaserDuration;
    [Header("レーザーがプレイヤーに与えるダメージ"), SerializeField] float LaserDamage;
    [Header("レーザーを弾速ありで扱う場合の速度"), SerializeField] float LaserSpeed;
    [Header("レーザーを即時判定の攻撃として扱うか"), SerializeField] bool UseInstantLaser = false;
    [Header("レーザーが壁に当たった場合反射するか"), SerializeField] bool UseWallReflection = true;
    [Header("レーザーが壁で反射できる最大回数"), SerializeField] int MaxReflectionCount;
    [Header("最大反射回数を超えたらレーザーを消滅させる"), SerializeField] bool DestroyOnReflectionLimit = true;
    [Header("レーザーの最大発射角度"), SerializeField] float MaxLesarAngle = 120;
    [Header("レーザー攻撃後、防壁を生成するか"), SerializeField] bool UseBarrier = true;
    [Header("防壁を設置するまでの待機時間"), SerializeField] float BarrierSpawnTiming;
    [Header("レーザーが防壁に当たった場合、屈折させるか"), SerializeField] bool UseBarrierRefraction;
    [Header("屈折後レーザーのダメージ"), SerializeField] int RefractionDamage;
    [Header("屈折後レーザーが壁でさらに反射するか"), SerializeField] bool RefractionCanReflectWall = true;
    [Header("一連の攻撃後、最初に戻るまでの待機時間"), SerializeField] float LoopDelayAfterAction;
    [Header("バリアのPrefab"), SerializeField] GameObject _barrierPrefab;
    [Header("バリアを生成する砲台との距離"), SerializeField] float _barrierSpawnDistance = 3;
    [Header("砲台の向き(右向きならtrue)"), SerializeField] bool _isFacingRight = true;
    [Header("屈折の強さ"), SerializeField] float _refractionStrength = 0.4f;
    float _remainingLength;//反射した後の残りの距離
    List<Vector3> points;//当たった場所を保持するList
    bool _isRefracted;//屈折後か
    Vector3 _playerDistance;
    Vector3 _laserDir;
    Transform _player;
    LineRenderer _lineRenderer;
    bool _isLaser;
    
    AttackLoop _currentState = AttackLoop.Laser;
    private enum AttackLoop
    {
        Laser,
        Barrier,
        Return
    }
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.enabled = false;
        _player = GameObject.FindGameObjectWithTag("Player").transform;
    }


    private bool IsPlayerInRange()//プレイヤーが範囲内か
    {
        Vector3 _toPlayer = _player.position - transform.position;

        float forwardDis = _isFacingRight ? _toPlayer.x : - _toPlayer.x;
        float verticalDis = Mathf.Abs(_toPlayer.y);

        bool inWidth = forwardDis >= 0 && forwardDis <= DetectWidth;
        bool inHeight = verticalDis <= DetectHeight / 2f;

        return inWidth && inHeight;
    }
    
    void Update()
    {
        if (IsPlayerInRange() && !_isLaser)
        {
            Attack();
        }
        else if(!_isLaser)
        {
            _lineRenderer.enabled = false;
            _currentState = AttackLoop.Laser;
        }
    }

    public IEnumerator LaserRoutine()
    {
        _isLaser = true;
        float Lasertimer = 0;
        yield return new WaitForSeconds(LaserStartDelay);
        bool isSameHeight = Mathf.Abs(_player.position.y - transform.position.y) < SameYThreshold;
        _playerDistance = _player.position - LaserOrigin.position;//レーザの発射口とプレイヤーの距離
        _playerDistance.z = 0f;
        if (isSameHeight)//プレイヤーが同じY座標だったら
        {
            _laserDir = _playerDistance.x > 0 ? Vector3.right : Vector3.left;
        }
        else
        {
            _laserDir = _playerDistance.normalized;
            // 発射角度を制限
            Vector3 forward = _isFacingRight ? Vector3.right : Vector3.left;
            float angleToPlayer = Vector3.Angle(forward, _laserDir);
            float halfAngle = MaxLesarAngle / 2f;

            if (angleToPlayer > halfAngle)
            {
                float sign = Mathf.Sign(_playerDistance.y);
                float clampedRad = halfAngle * Mathf.Deg2Rad * sign;
                float dirX = Mathf.Cos(clampedRad);
                float dirY = Mathf.Sin(clampedRad);
                _laserDir = _isFacingRight ? new Vector3(dirX, dirY, 0f) : new Vector3(-dirX, dirY, 0f);
            }
        }
        if (UseInstantLaser)//レーザーが即時判定の攻撃だったら
        #region
        {
            if (Physics.Raycast(LaserOrigin.position, _laserDir, out RaycastHit hit, LaserLength))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    //レーザーダメージ処理
                }
                DrawLaser(LaserOrigin.position, hit.point);
            }
            else
            {
                DrawLaser(LaserOrigin.position, LaserOrigin.position + _laserDir * LaserLength); //最大距離まで
            }
            yield return new WaitForSeconds(LaserDuration);
            _lineRenderer.enabled = false;
            _isLaser = false;
            _currentState = AttackLoop.Return;
        }
        #endregion//
        else
        {
            float currentLength = 0f;
            _remainingLength = LaserLength;
            int _reflectionCount = 0;
            Vector3 origin = LaserOrigin.position;
            points = new() { origin };//当たった場所を保持する
            _isRefracted = false;
            while (currentLength < _remainingLength && Lasertimer < LaserDuration)
            {
                currentLength += LaserSpeed * Time.deltaTime;
                Lasertimer += Time.deltaTime;
                if (Physics.Raycast(origin, _laserDir, out RaycastHit hit, currentLength))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        if(!_isRefracted)
                        {
                            //レーザーダメージ処理
                        }
                        else
                        {
                            //屈折ダメージ処理
                        }
                    }
                    else if (UseBarrierRefraction && hit.collider.GetComponent<EnemyBarrier>() != null)//当たった物にEnemyBarrierがついてるか
                    {
                        points.Add(hit.point);
                        _remainingLength -= currentLength; // 今まで進んだ分を引く
                        Vector3 refractionLaser = Vector3.Reflect(_laserDir, hit.normal);
                        _laserDir = Vector3.Lerp(_laserDir, refractionLaser, _refractionStrength).normalized;
                        origin = hit.point + _laserDir * 0.01f;   // 起点を更新(めり込み防止)
                        currentLength = 0f;
                        _isRefracted = true;
                    }
                    else
                    {
                        points.Add(hit.point);
                        DrawLaserPath(points.ToArray()); // 経路をまとめて描画
                        origin = hit.point;                // 次の発射点を更新
                        if (!UseWallReflection || isSameHeight || _reflectionCount >= MaxReflectionCount||(_isRefracted && !RefractionCanReflectWall))// 反射しない、Y座標が一緒、上限に達したら,
                        {
                            if (DestroyOnReflectionLimit)
                            {
                                _lineRenderer.enabled = false;
                            }
                            _isLaser = false;
                            _currentState = AttackLoop.Return;
                            yield break;
                        }
                        _remainingLength -= currentLength;
                        origin = hit.point;
                        _laserDir = Vector3.Reflect(_laserDir, hit.normal); // 新しい方向を計算
                        currentLength = 0f;
                        _reflectionCount++;                // 反射回数をカウントアップ
                    }
                }
                else
                {
                    Vector3 endPoint = origin + _laserDir * currentLength;
                    List<Vector3> tempPoints = new List<Vector3>(points) { endPoint };
                    DrawLaserPath(tempPoints.ToArray());
                }

                yield return null;
            }
            _lineRenderer.enabled = false;
            _isLaser = false;
            _currentState = AttackLoop.Return;
        }
    }
       private IEnumerator BarrierRoutine()
    {
        _isLaser = true;
        yield return new WaitForSeconds(BarrierSpawnTiming);
        Vector3 facingDir = _isFacingRight ? Vector3.right : Vector3.left;
        Vector3 barrierSpawnPos = transform.position + facingDir * _barrierSpawnDistance;
        var barrierobj = Instantiate(_barrierPrefab, barrierSpawnPos, Quaternion.identity);
        barrierobj.GetComponent<EnemyBarrier>().BarrierInit(facingDir);
        _currentState = AttackLoop.Laser;
        _isLaser = false;
    }
       private IEnumerator ReturnRoutine()
    {
        _isLaser = true;
        yield return new WaitForSeconds(LoopDelayAfterAction);
        _currentState = UseBarrier ? AttackLoop.Barrier : AttackLoop.Laser;
        _isLaser = false;
    }
  
        void DrawLaser(Vector3 start, Vector3 end)
        {
            _lineRenderer.enabled = true;
            _lineRenderer.SetPosition(0, start);
            _lineRenderer.SetPosition(1, end);
        }
        void DrawLaserPath(Vector3[] points)
        {
            _lineRenderer.enabled = true;
            _lineRenderer.positionCount = points.Length;
            _lineRenderer.SetPositions(points);
        }
    
    
    public IEnumerator StopLaserRoutine()
    {
        yield return new WaitForSeconds(LaserDuration);
        _lineRenderer.enabled = false;
    }
    protected override void Attack()
    {
        if (_isLaser) return;
        switch(_currentState)
        {
            case AttackLoop.Laser:
                StartCoroutine(LaserRoutine());
                    break;
            case AttackLoop.Barrier:
                StartCoroutine(BarrierRoutine());
                break;
            case AttackLoop.Return:
                StartCoroutine(ReturnRoutine());
                break;
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
