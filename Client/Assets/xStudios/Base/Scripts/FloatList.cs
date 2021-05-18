using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using XLua;

namespace X
{
    [LuaCallCSharp]
    [System.Serializable]
    public class FloatList
    {
        List<float> list = new List<float>();
        string split = ",";

        public List<float> List { get { return list; } }

        public float this[ int i ]
        {
            get
            {
                return list[ i ];
            }

            set
            {
                list[ i ] = value;
            }
        }

        public int Count { get { return list.Count; } }

        public void Clear()
        {
            list.Clear();
        }

        public void Init( string str , string s )
        {
            split = s;

            if ( string.IsNullOrEmpty( str ) )
            {
                return;
            }

            list.Clear();

            string[] arr = Regex.Split( str , split );

            for ( int i = 0 ; i < arr.Length ; i++ )
            {
                float f = 0f;
                Utility.ParseFloat( arr[ i ] , out f );

                list.Add( f );
            }
        }

        public void Add( float f )
        {
            list.Add( f );
        }

        public void Remove( float f )
        {
            list.Remove( f );
        }

        public void RemoveAt( int n )
        {
            list.RemoveAt( n );
        }

        public override string ToString()
        {
            string data = "";

            for ( int i = 0 ; i < list.Count ; i++ )
            {
                data += list[ i ].ToString();

                if ( i != list.Count - 1 )
                    data += split;
            }

            return data;
        }

    }
}

