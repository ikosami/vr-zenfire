using UnityEngine;
using System.Collections;

namespace NS.Util
{
    public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));
                    if (_instance == null)
                    {
#if UNITY_EDITOR
                        Debug.LogWarning(typeof(T) + " is nothing");
#endif
                    }
                }

                return _instance;
            }
        }

        protected virtual void Awake()
        {
            CheckInstance();
        }

        protected void CheckInstance()
        {
            if (this != Instance)
            {
                Destroy(this.gameObject);
#if UNITY_EDITOR
                Debug.Log(
                    typeof(T) +
                    " は既に他のGameObjectにアタッチされているため、コンポーネントを破棄しました." +
                    " アタッチされているGameObjectは " + Instance.gameObject.name + " です.");
#endif

                return;
            }
        }
    }

}