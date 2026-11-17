

using UnityEngine.SceneManagement;

public class SceneController : Singleton<SceneController>
{
    void SceneChanger(string name)
    {
        SceneManager.LoadScene(name);
    }
}
