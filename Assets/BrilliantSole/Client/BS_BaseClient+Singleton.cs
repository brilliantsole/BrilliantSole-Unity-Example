public abstract class BS_BaseClient<T> : BS_BaseClient where T : BS_BaseClient<T>, new()
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new T();
                //_instance.Initialize();
            }
            return _instance;
        }
    }

    public static void DestroyInstance()
    {
        //_instance?.DeInitialize();
        _instance = null;
    }

    protected BS_BaseClient() { }
}

