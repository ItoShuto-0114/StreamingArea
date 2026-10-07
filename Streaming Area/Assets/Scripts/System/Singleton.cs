using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (Instance == null)
                Debug.LogWarning($"Singleton{typeof(T)}のインスタンスが存在していません");

            return instance;
        }
    }

    protected void Awake()
    {
        if( instance == null )
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning($"Singleton{typeof(T)}が重複しました");
            Destroy(gameObject);
        }
    }
}
