using UnityEngine;
using UnityEngine.InputSystem;

public enum StyleType
{
    None,
    Speed,
    Blade,
    Unison,
    Berserk
}
[System.Serializable]
public struct StyleStats
{
    [Tooltip("速さの倍率")] public float SpeedRate;
    [Tooltip("攻撃力の倍率")] public float PowerRate;
    [Tooltip("受けるダメージの倍率")] public float TakeDamageRate;
    [Tooltip("攻撃速度の倍率")] public float AttackSpeedRate;
    [Tooltip("熱狂値の倍率")] public float FeverRate;
    [Tooltip("当たり判定の倍率")] public float HitBoxRate;
}
[System.Serializable]
public class PlayerStyle
{
    [Header("スピードシフト")]
    [SerializeField] StyleStats _speedShift = new() {SpeedRate = 1.2f,PowerRate = 1,TakeDamageRate = 0,AttackSpeedRate = 0,FeverRate = 0,HitBoxRate = 0};
    
    [Header("ブレードシフト")]
    [SerializeField] StyleStats _bladeShift = new() { SpeedRate = 1.2f, PowerRate = 1, TakeDamageRate = 0, AttackSpeedRate = 0, FeverRate = 0, HitBoxRate = 0 };

    [Header("ユニゾンシフト")]
    [SerializeField] StyleStats _unisonShift = new() { SpeedRate = 1.2f, PowerRate = 1, TakeDamageRate = 0, AttackSpeedRate = 0, FeverRate = 0, HitBoxRate = 0 };

    [Header("バーサクシフト")]
    [SerializeField] StyleStats _berserkShift = new() { SpeedRate = 1.2f, PowerRate = 1, TakeDamageRate = 0, AttackSpeedRate = 0, FeverRate = 0, HitBoxRate = 0 };

    public StyleType CurrentStyle { get; private set; } = StyleType.None;
    public void ChangeStyle()
    {
         switch(CurrentStyle)
        {
            case StyleType.None:
                CurrentStyle = StyleType.Speed;
                break;

            case StyleType.Speed:
                CurrentStyle = StyleType.Blade;
                break;

            case StyleType.Blade:
                CurrentStyle = StyleType.Unison;
                break;

            case StyleType.Unison:
                CurrentStyle = StyleType.Berserk;
                break;

            case StyleType.Berserk:
                CurrentStyle = StyleType.Speed;
                break;
        }
        Debug.Log(CurrentStyle);
    }
    public StyleStats GetStats()
    {
        switch(CurrentStyle)
        {
            case StyleType.Speed:
                return _speedShift;

            case StyleType.Blade:
                return _bladeShift;

            case StyleType.Unison:
                return _unisonShift;

            case StyleType.Berserk:
                return _berserkShift;
        }
        return new StyleStats { SpeedRate = 1, PowerRate = 1, TakeDamageRate = 1, AttackSpeedRate = 1, FeverRate = 1,HitBoxRate = 1 };
    }
}