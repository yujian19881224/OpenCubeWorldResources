using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace X
{
    [LuaCallCSharp]
    public class Asset
    {
        protected string path = "";

        public string Path { get { return path; } set { path = value; } }

        static public Asset NewSingleAsset()
        {
#if UNITY_EDITOR
            return new AssetEditor();
#else
            return new AssetRuntime();
#endif
        }

        static public Asset NewRuntimeAsset()
        {
            return new AssetRuntime();
        }

        public delegate void loadOver( Asset asset , bool success );
        public delegate void getAssetOver( UnityEngine.Object obj , bool success );

        public virtual void Clear() { }

        public virtual void Unload( bool child ) { }

        public virtual bool Load( string path ) { return false; }

        public virtual void LoadAsync( MonoBehaviour monoBehaviour , string path , loadOver over ) { }

        public virtual UnityEngine.Object GetAsset( string name ) { return null; }

        public virtual void GetAssetAsync( MonoBehaviour monoBehaviour , string name , getAssetOver over ) { }
    }
}
