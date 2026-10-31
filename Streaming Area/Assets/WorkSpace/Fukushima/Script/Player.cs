using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// プレイヤーの窓口。入力を受け取り、PlayerControllerに指示を出す
/// </summary>
public class Player : MonoBehaviour
{
    private PlayerController _controller;
    private int _priorityDir = 0;   // 優先する入力（-1：左 0：入力なし 1：右）

    void Start()
    {
        _controller = GetComponent<PlayerController>();
    }

    void Update()
    {
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