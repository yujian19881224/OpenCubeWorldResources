using UnityEngine;
using System.Collections;


namespace X
{
    public abstract class SingletonNew<T> where T : new()
    {
        static protected T instance = default( T );
        static public T Instance
        {
            get
            {
                if ( instance == null )
                {
                    instance = new T();
                }
                return instance;
            }
        }

    }

}
