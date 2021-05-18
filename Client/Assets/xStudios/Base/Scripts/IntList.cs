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
    public class IntList
    {
        List<int> list = new List<int>();
        string split = ",";

        public List<int> List { get { return list; } }

        public int this[ int i ]
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
            if ( !string.IsNullOrEmpty( s ) )
                split = s;

            if ( string.IsNullOrEmpty( str ) )
            {
                return;
            }

            list.Clear();

            string[] arr = Regex.Split( str , s );

            for ( int i = 0 ; i < arr.Length ; i++ )
            {
                int n = 0;
                Utility.ParseInt( arr[ i ] , out n );

                list.Add( n );
            }
        }

        public void Add( int n )
        {
            list.Add( n );
        }

        public void Remove( int n )
        {
            list.Remove( n );
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
