using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float PlayerSpeed;
    [SerializeField] private float PlayerJumpPower;
    [SerializeField] private float PlayerDashSpeed;
    [SerializeField] private float maxDistance = 1.2f;
    [SerializeField] private float _groundCheckDistance = 1.1f; // 足元を調べる距離

    private Rigidbody _rb;
    private int _moveDir = 0; // Playerから指示された移動方向（ -1：左 0：入力なし 1：右）

    [SerializeField] private float _waitInput = 0.1f;   // 壁キック後の待ち時間
    [SerializeField] private float _waitTimer = 0f;
    [SerializeField] private bool _isWallKick = false;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _waitTimer = 0f;
    }

    void Update()
    {
        // 壁キック直後は、待ち時間を数える
        if (_isWallKick)
        {
            _waitTimer += Time.deltaTime;

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
        Debug.DrawRay(transform.position, Vector3.down * _groundCheckDistance, Color.green);

        return Physics.Raycast(transform.position, Vector3.down, _groundCheckDistance, Physics.DefaultRaycastLayers);
    }

    /// <summary>
    /// 壁キック
    /// </summary>
    void WallKick()
    {
        // 左右どっち向きかを-1・1で取得
        Vector3 moveDir = (transform.right * _moveDir).normalized;

        // Rayを作成（原点: 自分の位置, 方向: 前方）
        Ray ray = new Ray(transform.position, moveDir);

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
            _rb.linearVelocity = (-moveDir + transform.up) * PlayerJumpPower;

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
