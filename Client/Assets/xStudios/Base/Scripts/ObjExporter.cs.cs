using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using XLua;

namespace X
{
    [LuaCallCSharp]
    public class ObjExporter
    {

        public static string MeshToString( MeshFilter mf )
        {
            Mesh m = mf.mesh;

            StringBuilder sb = new StringBuilder();

            sb.Append( "g " ).Append( mf.name ).Append( "\n" );
            foreach ( Vector3 v in m.vertices )
            {
                sb.Append( string.Format( "v {0} {1} {2}\n" , v.x , v.y , v.z ) );
            }
            sb.Append( "\n" );
            foreach ( Vector3 v in m.normals )
            {
                sb.Append( string.Format( "vn {0} {1} {2}\n" , v.x , v.y , v.z ) );
            }
            sb.Append( "\n" );
            foreach ( Vector3 v in m.uv )
            {
                sb.Append( string.Format( "vt {0} {1}\n" , v.x , v.y ) );
            }

            return sb.ToString();
        }

        public static void MeshToFile( MeshFilter mf , string filename )
        {
            using ( StreamWriter sw = new StreamWriter( filename ) )
            {
                sw.Write( MeshToString( mf ) );
            }
        }
    }

}
