using UnityEngine;
using System.Collections;

namespace X
{
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        static private T instance = null;
        static public T Instance
        {
            get
            {
                return instance;
            }
        }

        private void Awake()
        {
            if ( instance == null )
            {
                instance = this as T;
                instance.InitSingleton();
            }
        }

        protected virtual void InitSingleton()
        {

        }

    }


}
