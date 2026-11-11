using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameResult
{
    None,
    Clear,
    GameOver
}
public class GameManager : Singleton<GameManager>
{
    public GameResult Result { get; private set; } = GameResult.None;
    public void EndGame(GameResult result)
    {
        if (Result != GameResult.None) return;
        Result = result;
        EnemyManager.Instance?.ResetState();
        SceneChange(3); //ResultScene‚Ö
    }
    public void SceneChange(int scenenumber)
    {
        SceneManager.LoadScene(scenenumber);
        ResetResult();
    }
    public void ResetResult()
    {
        Result = GameResult.None;
    }
}
