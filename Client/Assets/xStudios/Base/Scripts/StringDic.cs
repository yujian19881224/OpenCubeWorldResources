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
    public class StringDictionary
    {
        Dictionary<string , string> dictionary = new Dictionary<string , string>();

        public Dictionary<string , string> Dictionary { get { return dictionary; } }

        public int Count { get { return dictionary.Count; } }

        public void Clear()
        {
            dictionary.Clear();
        }

        public void Add( string k , string v )
        {
            if ( dictionary.ContainsKey( k ) )
            {
                dictionary[ k ] = v;
            }
            else
            {
                dictionary.Add( k , v );
            }
        }

        public void Remove( string k )
        {
            dictionary.Remove( k );
        }

        public bool Contains( string k )
        {
            return dictionary.ContainsKey( k );
        }

        public string GetValue( string k )
        {
            string v = "";

            if ( dictionary.TryGetValue( k , out v ) )
            {
                return v;
            }

            return null;
        }
    }
}
