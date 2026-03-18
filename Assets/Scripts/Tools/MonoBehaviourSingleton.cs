using UnityEngine;

public abstract class MonoBehaviourSingleton<T> : MonoBehaviour where T : Component
{
    public static T Instance;

    protected bool SingletonCheck(T obj, bool ddol = true, bool fake = false)
    {
        if (!fake)
        {
            if (Instance == null)
            {
                Instance = obj;
                if (ddol) DontDestroyOnLoad(obj);

                return true;
            }
            else
            {
                Destroy(gameObject);
                return false;
            }
        }
        else
        {
            Instance = obj;
            return true;
        }
    }
}
