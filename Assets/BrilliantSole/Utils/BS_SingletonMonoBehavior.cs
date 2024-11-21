using UnityEngine;

public class BS_SingletonMonoBehavior<T> : MonoBehaviour where T : MonoBehaviour
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_SingletonMonoBehavior", BS_Logger.LogLevel.Log);

    private static T _instance;
    private static readonly object _lock = new();

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance = FindFirstObjectByType<T>();

                    if (_instance == null)
                    {
                        GameObject singletonObject = new(typeof(T).Name);
                        _instance = singletonObject.AddComponent<T>();
                        DontDestroyOnLoad(singletonObject);
                    }
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Logger.LogWarning($"Multiple instances of {typeof(T).Name} detected! Destroying duplicate.");
            Destroy(gameObject);
        }
    }
}