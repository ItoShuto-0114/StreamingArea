using UnityEngine;

public class ComboCounter
{
     UserData _data;
    float _maxFever;
    private float _currentScore;
    public ComboCounter(float max,UserData data)
    {
        _maxFever = max;
        _data = data;
    }
    public void AddFever(float value,string reason)
    {
        Debug.Log("data null? " + (_data == null));
        _currentScore = Mathf.Clamp(_currentScore + value,0, _maxFever);
        if (_data != null)
        {
            _data.CurrentFever = _currentScore;
            if (_currentScore > _data.MaxFever) _data.MaxFever = _currentScore;
        }
        Debug.Log(reason);
    }
}
