using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;

namespace X
{
    [LuaCallCSharp]
    public class AssetRuntime : Asset
    {
        AssetBundle assetBundle = null;
        bool isLoading = false;

        public override void Clear()
        {
            if ( assetBundle != null )
            {
                Unload( true );
            }
        }

        public override void Unload( bool child )
        {
            assetBundle.Unload( child );
            assetBundle = null;
        }

        public override bool Load( string p )
        {
            path = System.IO.Path.ChangeExtension( p , Utility.AssetExtension );

            if ( !File.Exists( path ) )
            {
                return false;
            }

            if ( assetBundle != null )
            {
                return true;
            }

            assetBundle = AssetBundle.LoadFromFile( path );

            return assetBundle != null;
        }

        public override void LoadAsync( MonoBehaviour monoBehaviour , string p , loadOver over )
        {
            if ( assetBundle != null ||
                isLoading )
            {
                over( this , assetBundle != null );
                return;
            }

            path = System.IO.Path.ChangeExtension( p , Utility.AssetExtension );

            isLoading = true;
            monoBehaviour.StartCoroutine( LoadAsync( path , over ) );
        }

        IEnumerator LoadAsync( string p , loadOver over )
        {
            AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync( p );
            yield return request;

            isLoading = false;

            assetBundle = request.assetBundle;

            over( this , assetBundle != null );
        }

        public override UnityEngine.Object GetAsset( string name )
        {
            if ( assetBundle == null )
            {
                return null;
            }

            return assetBundle.LoadAsset( name );
        }

        public override void GetAssetAsync( MonoBehaviour monoBehaviour , string name , getAssetOver over )
        {
            if ( assetBundle == null || 
                isLoading )
            {
                over( null , false );
                return;
            }

            isLoading = true;
            monoBehaviour.StartCoroutine( LoadAsync( name , over ) );
        }

        IEnumerator LoadAsync( string name , getAssetOver over )
        {
            AssetBundleRequest request = assetBundle.LoadAssetAsync( name );
            yield return request;

            isLoading = false;

            over( request.asset , request.asset != null );
        }

    }
}

