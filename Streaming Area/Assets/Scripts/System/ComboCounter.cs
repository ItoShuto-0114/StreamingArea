using UnityEngine;

public class ComboCounter
{
    //Userdata _data;
    float _maxFever;
    private float _currentScore;
    public ComboCounter(float max)//data‚ðŽæ“¾‚·‚é
    {
        _maxFever = max;
        //_data = data;
    }
    public void AddFever(float value,string reason)
    {
        _currentScore = Mathf.Clamp(_currentScore + value,0, _maxFever);
        Debug.Log(reason);
    }
}
