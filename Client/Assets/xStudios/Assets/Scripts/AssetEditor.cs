using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace X
{
    public class AssetEditor : Asset
    {
        UnityEngine.Object obj = null;
        bool isLoading = false;

        public override void Clear()
        {
            if ( obj != null )
            {
                Unload( true );
            }
        }

        public override void Unload( bool child )
        {
            obj = null;
        }

        public override bool Load( string p )
        {
            if ( obj != null )
            {
                return true;
            }

#if UNITY_EDITOR
            obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>( p );
#endif

            path = p;
            return obj != null;
        }

        public override void LoadAsync( MonoBehaviour monoBehaviour , string p , Asset.loadOver over )
        {
            if ( obj != null ||
                isLoading )
            {
                over( this , obj != null );
                return;
            }

            path = p;
            isLoading = true;
            monoBehaviour.StartCoroutine( LoadAsync( p , over ) );
        }

        IEnumerator LoadAsync( string path , Asset.loadOver over )
        {
            yield return new WaitForEndOfFrame();

#if UNITY_EDITOR
            obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>( path );
#endif

            isLoading = false;

            over( this , obj != null );
        }

        public override UnityEngine.Object GetAsset( string name )
        {
            return obj;
        }

        public override void GetAssetAsync( MonoBehaviour monoBehaviour , string name , Asset.getAssetOver over )
        {
            if ( obj == null ||
                isLoading )
            {
                over( obj , obj != null );
                return;
            }

            isLoading = true;
            monoBehaviour.StartCoroutine( LoadAsync( name , over ) );
        }

        IEnumerator LoadAsync( string name , Asset.getAssetOver over )
        {
            yield return new WaitForEndOfFrame();

            isLoading = false;

            over( obj , obj != null );
        }

    }
}

