using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// プレイヤーの窓口。入力を受け取り、PlayerControllerに指示を出す
/// </summary>
public class Player : MonoBehaviour
{
    private PlayerController _controller;

    void Start()
    {
        _controller = GetComponent<PlayerController>();
        Debug.Log("PlayerControllerを見つけた：" + (_controller != null));
    }

    void Update()
    {
        // スペースキーが押された瞬間にジャンプ処理をPlayerControllerに送る
        if(Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            _controller.Jump();
        }
    }
}