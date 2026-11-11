using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// プレイヤーの窓口。入力を受け取り、PlayerControllerに指示を出す
/// </summary>
public class Player : MonoBehaviour
{
    [Header("移動")]
    [SerializeField] private float _moveSpeed = 5f;          // 通常の移動速度
    [SerializeField] private float _jumpPower = 5f;          // ジャンプ力
    [SerializeField] private float _dashSpeed = 10f;         // ダッシュの速度

    [Header("判定")]
    [SerializeField] private float _wallCheckDistance = 1.2f;   // 壁を調べる距離
    [SerializeField] private float _groundCheckDistance = 1.1f; // 足元を調べる距離
    [SerializeField] private float _wallKickWaitTime = 0.1f;    // 壁キック後の待ち時間

    private PlayerController _controller;
    private int _priorityDir = 0;   // 優先する入力（-1：左 0：入力なし 1：右）

    void Start()
    {
        // ピュアクラスはGetComponentできないので、Playerが必要なものを集めて渡す
        Rigidbody rb = GetComponent<Rigidbody>();

        // PlayerControllerを new で作る（ここでコンストラクタが呼ばれる）
        _controller = new PlayerController(
            rb, transform,
            _moveSpeed, _jumpPower, _dashSpeed,
            _wallCheckDistance, _groundCheckDistance, _wallKickWaitTime);
    }

    void Update()
    {
        // 壁キック後の待ち時間などを進める
        _controller.Tick(Time.deltaTime);

        // 左右入力の優先を決定
        UpdatePriorityDir();

        // 移動
        if(Keyboard.current.shiftKey.isPressed)
        {
            _controller.Dash(_priorityDir);
        }
        else
        {
            _controller.Move(_priorityDir);
        }

        // スペースキーが押された瞬間にジャンプ処理をPlayerControllerに送る
        if(Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            _controller.Jump();
        }
    }

    /// <summary>
    /// 左右入力どちらを優先するか決める
    /// </summary>
    void UpdatePriorityDir()
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
            if (isLeftHeld)
            {
                _priorityDir = -1;
            }
            else if (isRightHeld)
            {
                _priorityDir = 1;
            }
        }
    }
}