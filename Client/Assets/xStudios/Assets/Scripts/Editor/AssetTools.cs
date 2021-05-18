using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace X
{
    public class AssetTools
    {
        
        [MenuItem( "XAssets/Setting" )]
        public static void Setting()
        {
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath( "Assets/xStudios/Assets/Resources/AssetSetting.asset" );
        }

        [MenuItem( "XAssets/ConvertConfig" )]
        public static void ConvertConfig()
        {
            AssetSetting assetSetting = Resources.Load<AssetSetting>( "AssetSetting" );

            for ( int i = 0 ; i < assetSetting.configPathList.Count ; i++ )
            {
                string path = assetSetting.configPathList[ i ];

                ConfigExporter.LoadSaveExcelAll( path );
            }

            Resources.UnloadAsset( assetSetting );
        }

    }

}
