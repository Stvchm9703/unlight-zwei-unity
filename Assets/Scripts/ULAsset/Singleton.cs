using System.Collections;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T> {
    #region Fields
    /// <summary>
    /// The instance.
    /// </summary>
    private static T instance = null;
    #endregion

    public static T Instance {
        get { return instance; }
    }

    public bool isPersistant;

    public virtual void Awake() {
        if (!instance) {
            instance = this as T;
        } else {
            Destroy(gameObject);
            // return ;
        }
        DontDestroyOnLoad(gameObject);
    }
}

/**

public abstract class Singleton<T> : MonoBehaviour where T : Component {
    #region Fields
    /// <summary>
    /// The instance.
    /// </summary>
    private static T instance;
    #endregion

    #region Properties
    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static T Instance {
        get {
            if (instance == null) {
                instance = FindObjectOfType<T>();
                if (instance == null) {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).Name;
                    instance = obj.AddComponent<T>();
                }
            }
            return instance;
        }
    }

    #endregion

    #region Methods
    /// <summary>
    /// Use this for initialization.
    /// </summary>
    protected virtual void Awake() {
        if (instance == null) {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    #endregion

}

*/