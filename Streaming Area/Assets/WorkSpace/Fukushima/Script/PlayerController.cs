using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float PlayerSpeed;
    [SerializeField] private float PlayerJumpPower;
    [SerializeField] private float PlayerDashSpeed;
    [SerializeField] private float maxDistance = 1.2f;
    [SerializeField] private float _groundCheckDistance = 1.1f; // 足元を調べる距離

    private Rigidbody _rb;
    private int _priorityDir = 0; // 優先する入力（ -1：左 0：入力なし 1：右）

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
        // 壁キック直後は何もしない
        if (_isWallKick && _waitInput > _waitTimer)
        {
            _waitTimer += Time.deltaTime;
            return;
        }
        else
        {
            Move();

            _isWallKick = false;
            _waitTimer = 0f;
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
        Vector3 moveDir = (transform.right * _priorityDir).normalized;

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
    /// 左右移動・ダッシュ
    /// </summary>
    void Move()
    {
        // 入力を取得
        bool isLeftHeld = Keyboard.current.aKey.isPressed;
        bool isRightHeld = Keyboard.current.dKey.isPressed;

        // 前の入力を判断
        // 左入力を優先
        if (_priorityDir == -1)
        {
            // 左入力優先状態かつ、左入力が離されたら
            if (!isLeftHeld)
            {
                // 右入力が押されていれば、右優先に切り替える
                if (isRightHeld)
                {
                    _priorityDir = 1;
                }
                else
                {
                    _priorityDir = 0;
                }
            }
        }
        else if (_priorityDir == 1)
        {
            // 右入力優先かつ、右入力が離されたら
            if (!isRightHeld)
            {
                // 左入力が押されていれば、左優先に切り替える
                if (isLeftHeld)
                {
                    _priorityDir = -1;
                }
                else
                {
                    _priorityDir = 0;
                }
            }
        }
        else if (_priorityDir == 0)
        {
            // どちらかの入力があれば、それを優先入力とする
            if (Keyboard.current.aKey.isPressed)
            {
                _priorityDir = -1;
            }
            else if (Keyboard.current.dKey.isPressed)
            {
                _priorityDir = 1;
            }
        }

        // 移動
        // Shiftキーを押しながらならダッシュ
        if (Keyboard.current.shiftKey.isPressed)
        {
            Dash();
        }
        else  // そうでないなら通常速度
        {
            _rb.linearVelocity = new Vector3(_priorityDir * PlayerSpeed, _rb.linearVelocity.y);
        }



    }

    /// <summary>
    /// ダッシュ
    /// </summary>
    void Dash()
    {
        _rb.linearVelocity = new Vector3(_priorityDir * PlayerDashSpeed, _rb.linearVelocity.y);
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
