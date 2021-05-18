using UnityEngine;
using System.Collections;
using System.IO;
using System;
using UnityEngine.Networking;


namespace X
{
    public abstract class SingletonManager<T> : MonoBehaviour where T : SingletonManager<T>
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
                DontDestroyOnLoad( gameObject );
                instance.InitSingleton();
            }
            else
            {
                Destroy( gameObject );
            }
        }

        protected virtual void InitSingleton()
        {

        }


    }


}
