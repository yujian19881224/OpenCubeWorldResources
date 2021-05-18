using UnityEngine;
using System.Collections;
using X;
using UnityEngine.EventSystems;
using XLua;
using System.Collections.Generic;

namespace X
{
    [LuaCallCSharp]
    public class AssetManager : Singleton<AssetManager>
    {
        Dictionary<string , Asset> assetDictionary = new Dictionary<string , Asset>();

        public void Unload( Asset asset )
        {
            asset.Clear();
            assetDictionary.Remove( asset.Path );
        }

        public Asset LoadSingle( string path )
        {
            Asset asset = null;

            if ( assetDictionary.TryGetValue( path , out asset ) )
            {
                return asset;
            }

            asset = Asset.NewSingleAsset();
            asset.Load( path );

            assetDictionary.Add( path , asset );

            return asset;
        }

        public Asset LoadRuntime( string path )
        {
            Asset asset = null;

            if ( assetDictionary.TryGetValue( path , out asset ) )
            {
                return asset;
            }

            asset = Asset.NewRuntimeAsset();
            asset.Load( path );

            assetDictionary.Add( path , asset );

            return asset;
        }

        public void Clear()
        {
            foreach ( KeyValuePair<string , Asset> item in assetDictionary )
            {
                item.Value.Clear();
            }

            assetDictionary.Clear();
        }
    }
}
