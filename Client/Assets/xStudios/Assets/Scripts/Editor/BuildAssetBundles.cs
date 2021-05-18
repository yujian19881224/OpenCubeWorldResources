using UnityEditor;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

namespace X
{
    public class BuildAssetBundles
    {
        public static string GetSelectedPathOrFallback()
        {
            string path = "Assets";

            foreach ( UnityEngine.Object obj in Selection.GetFiltered( typeof( UnityEngine.Object ) , SelectionMode.Assets ) )
            {
                path = AssetDatabase.GetAssetPath( obj );
                if ( !string.IsNullOrEmpty( path ) && File.Exists( path ) )
                {
                    path = Path.GetDirectoryName( path );
                    break;
                }
            }

            return path;
        }

        [MenuItem( "Assets/Build AssetBundles" )]
        static void BuildAllAssetBundles()
        {
            string path = "Assets";
            UnityEngine.Object[] objects = Selection.GetFiltered( typeof( UnityEngine.Object ) , SelectionMode.Assets );

            AssetSetting assetSetting = Resources.Load<AssetSetting>( "AssetSetting" );

            List<AssetBundleBuild> builds = new List<AssetBundleBuild>();
            AssetBundleBuild build = new AssetBundleBuild();
            build.assetBundleName = "assets" + Utility.AssetExtension;
            build.assetNames = new string[ objects.Length ];
            build.addressableNames = new string[ objects.Length ];

            for ( int i = 0 ; i < objects.Length ; i++ )
            {
                UnityEngine.Object obj = objects[ i ];

                path = AssetDatabase.GetAssetPath( obj );

                if ( !string.IsNullOrEmpty( path ) && File.Exists( path ) )
                {
                    string name = Path.GetFileNameWithoutExtension( path );

                    build.assetNames[ i ] = path;
                    build.addressableNames[ i ] = name;
                }
            }

            builds.Add( build );

            BuildPipeline.BuildAssetBundles( Path.GetDirectoryName( path ) , builds.ToArray() , 
                BuildAssetBundleOptions.None , assetSetting.buildTarget );

            Resources.UnloadAsset( assetSetting );
        }

        [MenuItem( "Assets/Build Single AssetBundles" )]
        static void BuildSingleAssetBundles()
        {
            string path = "Assets";
            UnityEngine.Object[] objects = Selection.GetFiltered( typeof( UnityEngine.Object ) , SelectionMode.Assets );

            AssetSetting assetSetting = Resources.Load<AssetSetting>( "AssetSetting" );

            for ( int i = 0 ; i < objects.Length ; i++ )
            {
                UnityEngine.Object obj = objects[ i ];

                path = AssetDatabase.GetAssetPath( obj );

                if ( !path.Contains( "." ) )
                {
                    continue;
                }

                if ( !string.IsNullOrEmpty( path ) && File.Exists( path ) )
                {
                    List<AssetBundleBuild> builds = new List<AssetBundleBuild>();
                    AssetBundleBuild build = new AssetBundleBuild();

                    string name = Path.GetFileNameWithoutExtension( path );

                    build.assetBundleName = path;
                    build.assetNames = new string[] { path };
                    build.addressableNames = new string[] { name };

                    builds.Add( build );

                    BuildPipeline.BuildAssetBundles( "Temp" , builds.ToArray() ,
                        BuildAssetBundleOptions.None , assetSetting.buildTarget );

                    FileInfo fi = new FileInfo( "Temp/" + path );
                    fi.MoveTo( Path.GetDirectoryName( path ) + "/" + name + Utility.AssetExtension );
                }
            }

            Resources.UnloadAsset( assetSetting );
        }
    }

}


