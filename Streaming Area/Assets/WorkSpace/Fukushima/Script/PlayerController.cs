using UnityEngine;

/// <summary>
/// プレイヤーの動きを担当するピュアクラス（何も継承しない）
/// Playerから new で作られ、呼ばれたときだけ動く
/// </summary>
public class PlayerController
{
    // Player.csから受け取る
    private Rigidbody _rb;
    private Transform _transform;

    private float PlayerSpeed;
    private float PlayerJumpPower;
    private float PlayerDashSpeed;
    private float maxDistance;          // 壁を調べる距離
    private float _groundCheckDistance; // 足元を調べる距離
    private float _waitInput;           // 壁キック後の待ち時間

    // 状態
    private int _moveDir = 0;           // Playerから指示された移動方向（-1：左 0：入力なし 1：右）
    private float _waitTimer = 0f;
    private bool _isWallKick = false;

    /// <summary>
    /// コンストラクタ（new されたときに1回だけ呼ばれる）
    /// Playerから受け取ったものを、自分の変数にしまっておく
    /// </summary>
    public PlayerController(Rigidbody rb, Transform transform,
        float moveSpeed, float jumpPower, float dashSpeed,
        float wallCheckDistance, float groundCheckDistance, float waitInput)
    {
        _rb = rb;
        _transform = transform;

        PlayerSpeed = moveSpeed;
        PlayerJumpPower = jumpPower;
        PlayerDashSpeed = dashSpeed;
        maxDistance = wallCheckDistance;
        _groundCheckDistance = groundCheckDistance;
        _waitInput = waitInput;
    }

    /// <summary>
    /// 毎フレームPlayerから呼ばれる（壁キック後の待ち時間を数える）
    /// </summary>
    public void Tick(float deltaTime)
    {
        // 壁キック直後は、待ち時間を数える
        if (_isWallKick)
        {
            _waitTimer += deltaTime;

            // 待ち時間が過ぎたら、また操作できるようにする
            if (_waitTimer >= _waitInput)
            {
                _isWallKick = false;
                _waitTimer = 0f;
            }
        }

    }

    /// <summary>
    /// 足元にRayを飛ばして、地面があるか調べる
    /// </summary>
    bool IsGrounded()
    {
        // Sceneビューで確認するためのデバッグ線（緑）
        Debug.DrawRay(_transform.position, Vector3.down * _groundCheckDistance, Color.green);

        return Physics.Raycast(_transform.position, Vector3.down, _groundCheckDistance, Physics.DefaultRaycastLayers);
    }

    /// <summary>
    /// 壁キック
    /// </summary>
    void WallKick()
    {
        // 左右どっち向きかを-1・1で取得
        Vector3 moveDir = (_transform.right * _moveDir).normalized;

        // Rayを作成（原点: 自分の位置, 方向: 前方）
        Ray ray = new Ray(_transform.position, moveDir);

        // ヒット情報を格納する変数
        RaycastHit hit;

        // Rayを飛ばす（ヒットした場合はtrue）
        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            // 当たったオブジェクトのGameObjectを取得
            GameObject hitObject = hit.collider.gameObject;

            Debug.Log("当たったオブジェクト: " + hitObject.name);

            // このメソッドに入った段階でジャンプ入力がされているため、ここでの入力受付は必要なし
            // 実際に力を加える
            _rb.linearVelocity = (-moveDir + _transform.up) * PlayerJumpPower;

            // 壁キック判定を真に
            _isWallKick = true;

        }

        // 視覚的に確認するためのデバッグ線（Sceneビューに表示）
        Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.red);
    }

    /// <summary>
    /// 左右移動（Player.csから呼ばれる）
    /// </summary>
    public void Move(int dir)
    {
        // 壁キック直後は何もしない
        if(_isWallKick)
        {
            return;
        }

        _moveDir = dir;
        _rb.linearVelocity = new Vector3(_moveDir * PlayerSpeed, _rb.linearVelocity.y);

    }

    /// <summary>
    /// ダッシュ（Player.csから呼ばれる）
    /// </summary>
    public void Dash(int dir)
    {
        // 壁キック直後は何もしない
        if (_isWallKick)
        {
            return;
        }

        _moveDir = dir;
        _rb.linearVelocity = new Vector3(_moveDir * PlayerDashSpeed, _rb.linearVelocity.y);
    }

    /// <summary>
    /// ジャンプ処理（Player.csから呼ばれる）
    /// </summary>
    public void Jump()
    {
        // 壁キック直後は何もしない
        if (_isWallKick)
            return;

        if (IsGrounded())
        {
            // 地面にいればジャンプ
            _rb.AddForce(new Vector3(0, PlayerJumpPower, 0), ForceMode.Impulse);
        }
        else
        {
            // 空中なら壁キックを試す
            WallKick();
        }
    }

}
