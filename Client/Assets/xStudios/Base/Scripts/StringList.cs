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
    public class StringList
    {
        List<string> list = new List<string>();
        string split = ",";

        public List<string> List { get { return list; } }


        public string this[ int i ]
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

            string[] arr = Regex.Split( str , s );

            for ( int i = 0 ; i < arr.Length ; i++ )
            {
                list.Add( arr[ i ] );
            }
        }

        public void Add( string s )
        {
            list.Add( s );
        }

        public void Remove( string s )
        {
            list.Remove( s );
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
