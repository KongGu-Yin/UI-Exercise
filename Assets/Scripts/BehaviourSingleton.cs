/// <summary>
/// Generic Mono singleton.
/// </summary>
using UnityEngine;

public abstract class BehaviourSingleton<T> : MonoBehaviour where T : BehaviourSingleton<T>
{

    private static T m_Instance = null;

    public static T instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = GameObject.FindObjectOfType(typeof(T)) as T;
                if (m_Instance == null)
                {
                    GameObject sinObj = GameObject.Find("SingleMonoBehaviour");
                    if (sinObj == null)
                        m_Instance = new GameObject("SingleMonoBehaviour").AddComponent<T>();
                    else
                        m_Instance = sinObj.AddComponent<T>();
                    DontDestroyOnLoad(m_Instance);
                    m_Instance.Init();
                }
            }
            return m_Instance;
        }
    }

    private void Awake()
    {

        if (m_Instance == null)
        {
            m_Instance = this as T;
            DontDestroyOnLoad(m_Instance);
        }
    }

    public virtual void Init() { }


    private void OnApplicationQuit()
    {
        m_Instance = null;
    }
}