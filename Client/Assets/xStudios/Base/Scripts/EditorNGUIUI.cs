using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


namespace X
{
    public abstract class EditorNGUIUI<T> : MonoBehaviour where T : EditorNGUIUI<T>
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

            gameObject.SetActive( false );
        }

        protected virtual void InitSingleton()
        {

        }

        [SerializeField]
        protected bool isShow = false;

        public bool IsShow { get { return isShow; } }


        public void Show()
        {
            if ( !isShow )
            {
                gameObject.SetActive( true );
                isShow = true;

                OnShow();
            }
        }

        public void UnShow()
        {
            if ( isShow )
            {
                gameObject.SetActive( false );
                isShow = false;

                OnUnShow();
            }
        }

        public virtual void OnShow()
        {

        }
        public virtual void OnUnShow()
        {

        }


    }

}
