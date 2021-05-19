using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace X
{
    public class AssetEditorTools
    {
        static List<string> hashList = new List<string>();

        

        static void CopyDirectory( string srcDir , string desDir )
        {
            string desfolderdir = desDir;

            string[] filenames = Directory.GetFileSystemEntries( srcDir );

            foreach ( string file in filenames )
            {
                if ( Directory.Exists( file ) )
                {
                    string currentdir = desfolderdir + file.Substring( file.LastIndexOf( "/" ) + 1 );
                    if ( !Directory.Exists( currentdir ) )
                    {
                        Directory.CreateDirectory( currentdir );
                    }

                    CopyDirectory( file , desfolderdir );
                }
                else
                {
                    string srcfileName = file.Substring( file.LastIndexOf( "/" ) + 1 );

                    if ( srcfileName.Contains( ".meta" ) )
                    {
                        continue;
                    }
                    if ( srcfileName.Contains( ".prefab" ) )
                    {
                        continue;
                    }

//                     if ( !srcfileName.Contains( ".lua" ) &&
//                         !srcfileName.Contains( ".cfg" ) &&
//                         !srcfileName.Contains( ".txt" ) &&
//                         !srcfileName.Contains( ".unity3d" ) &&
//                         !srcfileName.Contains( ".png" ) )
//                     {
//                         continue;
//                     }

                    hashList.Add( file );

                    srcfileName = desfolderdir + srcfileName;
                    if ( File.Exists( srcfileName ) )
                    {
                        File.Delete( srcfileName );
                    }

                    if ( !Directory.Exists( desfolderdir ) )
                    {
                        Directory.CreateDirectory( desfolderdir );
                    }

                    File.Copy( file , srcfileName );
                }
            }
        }

        [MenuItem( "XAssets/CopyMods" )]
        static void CopyMods()
        {
            hashList.Clear();

            string srcDir = Utility.AssetsPath + Utility.ModsPath;
            string desDir = Utility.BinPath + Utility.ModsPath;

            CopyDirectory( srcDir , desDir );
//             Utility.BuildHash( Utility.VersionsPath + "ModsHash.dat" , hashList );

            Debug.Log( "copy " + srcDir + " to " + desDir );
        }

//         [MenuItem( "XAssets/CopyMaps" )]
//         static void CopyMaps()
//         {
//             SaveMapInfos();
// 
//             hashList.Clear();
// 
//             string srcDir = Utility.MapsPath;
//             string desDir = Utility.BinPath + Utility.MapsPath;
// 
//             CopyDirectory( srcDir , desDir );
//             Utility.BuildHash( Utility.VersionsPath + "MapsHash.dat" , hashList );
// 
//             Debug.Log( "copy " + srcDir + " to " + desDir );
//         }


        public static MapInfo LoadMapInfo( string path )
        {
            FileStream fs = null;
            BinaryReader br = null;

            Utility.ClearMapTemp();

            string tempPath = Utility.MapsPath + Utility.TempPath;

            if ( !Utility.UnZipFile( path , tempPath ) )
            {
                return null;
            }

            MapInfo mapInfo = new MapInfo();

            try
            {
                fs = File.Open( tempPath + Utility.MapDataFile , FileMode.Open , FileAccess.ReadWrite , FileShare.ReadWrite );
                br = new BinaryReader( fs );
                string magic = br.ReadString();
                int version = br.ReadInt32();

                MapData mapData = mapInfo.MapData;

                mapData.Name = br.ReadString();
                mapData.Des = br.ReadString();
                mapData.Width = br.ReadInt16();
                mapData.Height = br.ReadInt16();
                mapData.MaxPlayer = br.ReadInt16();
                mapData.MaxWatcher = br.ReadInt16();
            }
            catch ( System.Exception e )
            {
                mapInfo = null;
            }
            finally
            {
                if ( fs != null )
                {
                    fs.Close();
                    fs.Dispose();
                }
                if ( br != null )
                {
                    br.Close();
                    br.Dispose();
                }
            }


            try
            {
                fs = File.Open( path , FileMode.Open , FileAccess.ReadWrite , FileShare.ReadWrite );

                byte[] bytes = new byte[ fs.Length ];
                fs.Read( bytes , 0 , (int)fs.Length );

                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] encryptdata = md5.ComputeHash( bytes );

                mapInfo.Hash = System.Convert.ToBase64String( encryptdata );
            }
            catch ( System.Exception e )
            {
            }
            finally
            {
                if ( fs != null )
                {
                    fs.Close();
                    fs.Dispose();
                }
            }

            mapInfo.Path = path;

#if UNITY_EDITOR
            mapInfo.File = path.Replace( Utility.MapsPath , "" );
#else
            mapInfo.File = path.Replace( Utility.CurrentDirectory + Utility.MapsPath , "" );
#endif

            return mapInfo;
        }

        public static void SaveMapInfo()
        {
            string[] maps = Utility.GetMaps();

            MapInfo[] mapInfos = new MapInfo[ maps.Length ];

            for ( int i = 0 ; i < maps.Length ; i++ )
            {
                mapInfos[ i ] = LoadMapInfo( maps[ i ] );
            }

            Utility.ClearMapTemp();


            FileStream fs = File.Open( "Bin/" + Utility.MapsPath + Utility.MapInfosFile , FileMode.Create , FileAccess.ReadWrite , FileShare.ReadWrite );

            BinaryWriter bf = new BinaryWriter( fs );
            bf.Write( "XMIS" );
            bf.Write( 1 );

            bf.Write( mapInfos.Length );

            for ( int i = 0 ; i < mapInfos.Length ; i++ )
            {
                MapInfo mapInfo = mapInfos[ i ];

                bf.Write( mapInfo.Hash );
                bf.Write( mapInfo.File );
                bf.Write( mapInfo.Path );

                bf.Write( mapInfo.MapData.Name );
                bf.Write( mapInfo.MapData.Des );
                bf.Write( mapInfo.MapData.Width );
                bf.Write( mapInfo.MapData.Height );
                bf.Write( mapInfo.MapData.MaxPlayer );
                bf.Write( mapInfo.MapData.MaxWatcher );
            }

            bf.Close();
            bf.Dispose();

            fs.Close();
            fs.Dispose();
        }

        [MenuItem( "XAssets/SaveMapInfos" )]
        static void SaveMapInfos()
        {
            SaveMapInfo();
            Debug.Log( "mapInfos saved.  " );
        }


//         [MenuItem( "XAssets/CreateCollider" )]
//         static void CreateCollider()
//         {
//             Object[] objects = Selection.objects;
// 
//             for ( int i = 0 ; i < objects.Length ; i++ )
//             {
//                 string path = AssetDatabase.GetAssetPath( objects[ i ] );
// 
//                 string name = Path.GetDirectoryName( path ) + "\\" + Path.GetFileNameWithoutExtension( path );
//                 name += "Collider.prefab";
// 
//                 GameObject gameObject = new GameObject();
//                 gameObject.layer = 8;
// 
//                 PrefabUtility.SaveAsPrefabAsset( gameObject , name );
// 
//                 GameObject.DestroyImmediate( gameObject );
//             }
//         }

        [MenuItem( "XAssets/Run" )]
        static void Run()
        {
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WorkingDirectory = System.Environment.CurrentDirectory + "/Bin/";
            startInfo.FileName = System.Environment.CurrentDirectory + "/Bin/CubeWorld.exe";

            System.Diagnostics.Process.Start( startInfo );
        }

    }
}

