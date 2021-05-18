using UnityEngine;
using System.Collections;

namespace X
{
    public abstract class Scene<T> : MonoBehaviour where T : Scene<T>
    {
        static private T instance = null;
        static public T Instance
        {
            get
            {
                return instance;
            }
        }

        public delegate void OnUpdate();
        OnUpdate onUpdate = null;

        public OnUpdate LuaUpdate { get { return onUpdate; } set { onUpdate = value; } }


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

        public virtual void LoadScene()
        {

        }

        public virtual void UnloadScene()
        {

        }


        private void Update()
        {
            if ( onUpdate == null )
                return;

            onUpdate();
        }


    }


}
