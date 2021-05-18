using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

namespace X
{
    public class PHP : Singleton<PHP>
    {
        public string GetHex( byte[] bytes )
        {
            StringBuilder ret = new StringBuilder();

            foreach ( byte b in bytes )
            {
                ret.AppendFormat( "{0:x2}" , b );
            }

            return ret.ToString();
        }

        public byte[] GetByte( string hex )
        {
            byte[] bytes = new byte[ hex.Length / 2 ];
            for ( int x = 0 ; x < bytes.Length ; x++ )
            {
                int i = Convert.ToInt32( hex.Substring( x * 2 , 2 ) , 16 );
                bytes[ x ] = (byte)i;
            }

            return bytes;
        }

        string url = "http://art.foxgames.cn/sqlRequestTest.php?sql=";
        string urlLog = "http://art.foxgames.cn/sqlRequestTestLog.php?str=";

        public delegate void phpCallback( int i , byte[] data );


        public void SaveLog( string str )
        {
#if !UNITY_EDITOR
            string urlPost = urlLog + SystemInfo.deviceUniqueIdentifier + "\r\n" + str + "\r\n\r\n";

            StartCoroutine( Post( urlPost , 0 , null ) );
#endif
        }

        public void Save( int i , byte[] dat , phpCallback cb )
        {
            string id = SystemInfo.deviceUniqueIdentifier;
            string d = GetHex( dat );

            string urlSql = url + "CALL .UPDATE_SAVE( '" + id + "' , '" + i + "' , '" + d + "' );";

            StartCoroutine( Post( urlSql , i , cb ) );
        }

        public void Load( int i , phpCallback cb )
        {
            string id = SystemInfo.deviceUniqueIdentifier;

            string urlSql = url + "CALL .SELECT_SAVE( '" + id + "' , '" + i + "' );";

            StartCoroutine( Get( urlSql , i , cb ) );
        }


        IEnumerator Post( string sql , int i , phpCallback cb )
        {
            UnityWebRequest www = UnityWebRequest.Get( sql );
            yield return www.SendWebRequest();

            if ( www.isNetworkError || www.isHttpError )
            {
#if UNITY_EDITOR
            Debug.Log( www.error );
#endif
            }
            else
            {
#if UNITY_EDITOR
            Debug.Log( www.downloadHandler.text );
#endif
                // Or retrieve results as binary data
                byte[] results = www.downloadHandler.data;

                try
                {
                    if ( cb != null )
                    {
                        cb( i , results );
                    }
                }
                catch ( Exception )
                {
                }
            }

        }

        IEnumerator Get( string sql , int i , phpCallback cb )
        {
            UnityWebRequest www = UnityWebRequest.Get( sql );
            yield return www.SendWebRequest();

            if ( www.isNetworkError || www.isHttpError )
            {
#if UNITY_EDITOR
            Debug.Log( www.error );
#endif
                //            Application.Quit();
            }
            else
            {
                byte[] bytes = GetByte( www.downloadHandler.text );

                try
                {
                    if ( cb != null )
                    {
                        cb( i , bytes );
                    }
                }
                catch ( Exception )
                {
                }
            }

        }




    }
}

