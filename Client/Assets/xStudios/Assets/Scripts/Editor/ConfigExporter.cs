using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace X
{
    public class TableCell
    {
        public string value = "";
        public ICellStyle style;

        public override string ToString()
        {
            if ( string.IsNullOrEmpty( value ) )
                value = "";

            return value;
        }
    }

    [Serializable]
    public class TableData
    {
        public string path = "";
        public string name = "";
        public string version = "";

        public string primaryKey = "";
        public int primaryKeyIndex = -1;

        public int headIndex = -1;
        public int scriptIndex = -1;
        public int formatIndex = -1;
        public int typeIndex = -1;
        public int keyIndex = -1;
        public int defaultIndex = -1;

        public System.Data.DataTable dataTable = new System.Data.DataTable();
        public List<string[]> data = new List<string[]>();
    }

    public class ConfigExporter
    {
        static public string log = "";
        static public int errorCount = 0;
        static public List<TableData> dataList = new List<TableData>();

        static public void Clear()
        {
            log = "";

            dataList.Clear();
            errorCount = 0;
        }

        static public void Log( string str )
        {
            UnityEngine.Debug.Log( str );
            log += str + "\r\n";
        }

        static public void LogError( string str )
        {
            UnityEngine.Debug.LogError( str );
            log += str + "\r\n";
            errorCount++;
        }

        static public void CloneStyle( ICellStyle from , ICellStyle to )
        {
            to.CloneStyleFrom( from );
        }

        static public void SaveExcel( string path , System.Data.DataTable dataTable )
        {
            try
            {
                Dictionary<ICellStyle , ICellStyle> dic = new Dictionary<ICellStyle , ICellStyle>();
                IWorkbook workbook = new XSSFWorkbook();
                int n = workbook.NumCellStyles;

                ISheet sheet = workbook.CreateSheet( dataTable.TableName );

                for ( int i = 0 ; i < dataTable.Rows.Count ; i++ )
                {
                    IRow row = sheet.CreateRow( i );
                    for ( int j = 0 ; j < dataTable.Columns.Count ; j++ )
                    {
                        TableCell dataTableCell = dataTable.Rows[ i ][ j ] as TableCell;

                        ICell cell = row.CreateCell( j );

                        if ( dataTableCell != null )
                        {
                            cell.SetCellValue( dataTableCell.value );

                            if ( dataTableCell.style != null )
                            {
                                ICellStyle cellStyle = null;
                                if ( !dic.TryGetValue( dataTableCell.style , out cellStyle ) )
                                {
                                    cellStyle = workbook.CreateCellStyle();
                                    dic.Add( dataTableCell.style , cellStyle );
                                    CloneStyle( dataTableCell.style , cellStyle );
                                }

                                cell.CellStyle = cellStyle;
                            }
                        }
                    }

                    ICell cellEnd = row.CreateCell( dataTable.Columns.Count );
                    cellEnd.SetCellValue( "" );

                    if ( dataTable.Rows[ i ][ 0 ].ToString() == "Key" ||
                        dataTable.Rows[ i ][ 0 ].ToString() == "Version" ||
                        dataTable.Rows[ i ][ 0 ].ToString() == "Head" ||
                        dataTable.Rows[ i ][ 0 ].ToString() == "Des" ||
                        dataTable.Rows[ i ][ 0 ].ToString() == "Script" ||
                        dataTable.Rows[ i ][ 0 ].ToString() == "Format" ||
                        dataTable.Rows[ i ][ 0 ].ToString() == "Type" ||
                        dataTable.Rows[ i ][ 0 ].ToString() == "Key" )
                    {
                        cellEnd.SetCellValue( "End" );
                        cellEnd.CellStyle = row.Cells[ 0 ].CellStyle;
                    }
                }

                IRow rowEnd = sheet.CreateRow( dataTable.Rows.Count );
                rowEnd.CreateCell( 0 ).SetCellValue( "End" );

                MemoryStream stream = new MemoryStream();
                workbook.Write( stream );
                var buf = stream.ToArray();

                Directory.CreateDirectory( Path.GetDirectoryName( path ) );

                FileStream fs = new FileStream( path , FileMode.Create , FileAccess.Write , FileShare.ReadWrite );
                fs.Write( buf , 0 , buf.Length );
                fs.Close();
                fs.Dispose();

                Log( "excell saved " + path );
            }
            catch ( Exception )
            {
                LogError( "save excell failed " + path );
            }
        }

        static public void SaveData( string path , TableData data )
        {
            path = path + data.name + ".cfg";

            FileStream fs = null;

            try
            {
                fs = File.Open( path , FileMode.Create , FileAccess.ReadWrite , FileShare.ReadWrite );

                BinaryWriter bf = new BinaryWriter( fs );
                bf.Write( "XCFG" );
                bf.Write( 1 );

                bf.Write( data.name );
                bf.Write( data.version );
                bf.Write( data.primaryKey );
                bf.Write( data.primaryKeyIndex );

                bf.Write( data.headIndex );
                bf.Write( data.scriptIndex );
                bf.Write( data.formatIndex );
                bf.Write( data.typeIndex );
                bf.Write( data.keyIndex );
                bf.Write( data.defaultIndex );

                int tableCount = 0;
                if ( data.headIndex != -1 )
                    tableCount++;
                if ( data.scriptIndex != -1 )
                    tableCount++;
                if ( data.formatIndex != -1 )
                    tableCount++;
                if ( data.typeIndex != -1 )
                    tableCount++;
                if ( data.keyIndex != -1 )
                    tableCount++;
                if ( data.defaultIndex != -1 )
                    tableCount++;

                bf.Write( tableCount );
                bf.Write( data.data.Count - tableCount );

                for ( int i = 0 ; i < data.data.Count ; i++ )
                {
                    bf.Write( data.data[ i ].Length );
                    for ( int j = 0 ; j < data.data[ i ].Length ; j++ )
                    {
                        bf.Write( data.data[ i ][ j ] );
                    }
                }

                bf.Close();
                bf.Dispose();

                Log( "save config " + path );
            }
            catch ( Exception e )
            {
                Log( "save excel error. " + path );
                LogError( e.Message );
            }
            finally
            {
                if ( fs != null )
                {
                    fs.Close();
                    fs.Dispose();
                }
            }
        }

        static public void LoadExcel( IWorkbook workbook )
        {
            int columnCount = 0;
            int sheetIndex = 0;

            int n = workbook.NumCellStyles;

            while ( workbook.NumberOfSheets > sheetIndex )
            {
                ISheet sheet = workbook.GetSheetAt( sheetIndex );
                sheetIndex++;

                TableData data = new TableData();
                data.name = sheet.SheetName;
                data.dataTable.TableName = data.name;

                dataList.Add( data );


                int rowIndex = 0;
                List<int> Remark = new List<int>();

                foreach ( IRow row in sheet )
                {
                    rowIndex++;

                    if ( row.Cells.Count == 0 )
                    {
                        continue;
                    }

                    bool End = false;
                    bool needContinue = false;

                    bool Version = false;
                    bool Head = false;
                    bool Script = false;
                    bool Format = false;
                    bool Type = false;
                    bool Key = false;
                    bool Default = false;

                    for ( int i = 0 ; i < row.Cells.Count ; i++ )
                    {
                        ICell cell = row.Cells[ i ];
                        string cellValue = cell.ToString();

                        if ( string.IsNullOrEmpty( cellValue ) )
                        {
                            continue;
                        }

                        if ( cell.ColumnIndex == 0 )
                        {
                            if ( cellValue == ";" )
                            {
                                needContinue = true;
                                break;
                            }
                            if ( cellValue == "Des" )
                            {
                                needContinue = true;
                                break;
                            }
                            else if ( cellValue == "End" )
                            {
                                End = true;
                                break;
                            }
                            else if ( cellValue == "Version" )
                            {
                                Version = true;
                            }
                            else if ( cellValue == "Head" )
                            {
                                Head = true;
                            }
                            else if ( cellValue == "Script" )
                            {
                                Script = true;
                            }
                            else if ( cellValue == "Format" )
                            {
                                Format = true;
                            }
                            else if ( cellValue == "Type" )
                            {
                                Type = true;
                            }
                            else if ( cellValue == "Key" )
                            {
                                Key = true;
                            }
                            else if ( cellValue == "Default" )
                            {
                                Default = true;
                            }
                        }
                        else
                        {
                            if ( cellValue == "End" )
                            {
                                columnCount = cell.ColumnIndex;
                                break;
                            }

                            if ( Version )
                            {
                                if ( cell.ColumnIndex == 1 )
                                {
                                    data.version = cell.ToString();
                                    break;
                                }
                            }

                            if ( Type )
                            {
                                if ( cellValue == "Int" || cellValue == "Float" ||
                                    cellValue == "Bool" || cellValue == "String" ||
                                    cellValue == "IntList" || cellValue == "FloatList" ||
                                    cellValue == "StringList" )
                                {
                                }
                                else
                                {
                                    LogError( "type error. " + cellValue + " row = " + rowIndex + "column = " + cell.ColumnIndex );
                                }
                            }

                            if ( Head )
                            {
                                if ( cellValue == "PrimaryKey" )
                                {
                                    data.primaryKeyIndex = cell.ColumnIndex - 1;

                                    for ( int k = 0 ; k < Remark.Count ; k++ )
                                        if ( Remark[ k ] < data.primaryKeyIndex )
                                            data.primaryKeyIndex--;
                                }
                                else if ( cellValue == "Remark" )
                                {
                                    Remark.Add( cell.ColumnIndex );
                                }
                                else if ( cellValue.Contains( "Key," ) )
                                {

                                }
                                else if ( cellValue.Contains( "Value," ) )
                                {

                                }
                                else
                                {
                                    LogError( "head error. " + cellValue + " row = " + rowIndex + "column = " + cell.ColumnIndex );
                                }
                            }

                            if ( Key )
                            {
                                int index = cell.ColumnIndex - 1;
                                for ( int k = 0 ; k < Remark.Count ; k++ )
                                    if ( Remark[ k ] < index )
                                        index--;

                                if ( index == data.primaryKeyIndex )
                                {
                                    data.primaryKey = cellValue;
                                }
                            }
                        }
                    }

                    if ( End )
                    {
                        break;
                    }

                    if ( needContinue )
                    {
                        continue;
                    }

                    if ( columnCount == 0 )
                    {
                        continue;
                    }

                    if ( Head )
                    {
                        data.headIndex = data.data.Count;
                    }
                    if ( Script )
                    {
                        data.scriptIndex = data.data.Count;
                    }
                    if ( Format )
                    {
                        data.formatIndex = data.data.Count;
                    }
                    if ( Type )
                    {
                        data.typeIndex = data.data.Count;
                    }
                    if ( Key )
                    {
                        data.keyIndex = data.data.Count;
                    }
                    if ( Default )
                    {
                        data.defaultIndex = data.data.Count;
                    }

                    string[] dd = new string[ columnCount - 1 - Remark.Count ];

                    for ( int i = 0 ; i < dd.Length ; i++ )
                        dd[ i ] = "";

                    for ( int i = 0 ; i < row.Cells.Count ; i++ )
                    {
                        int r = 0;
                        int index = row.Cells[ i ].ColumnIndex;

                        if ( Key )
                        {
                            if ( index < columnCount )
                            {
                                data.dataTable.Columns.Add( row.Cells[ i ].ToString() ,
                                    typeof( TableCell ) );
                            }
                        }

                        if ( index == 0 )
                            continue;

                        for ( int k = 0 ; k < Remark.Count ; k++ )
                            if ( index > Remark[ k ] )
                                r++;

                        if ( index < columnCount )
                        {
                            try
                            {
                                dd[ index - 1 - r ] = row.Cells[ i ].ToString();
                            }
                            catch ( Exception )
                            {
                                LogError( "data convert error. " + " row = " + rowIndex + "column = " + index );
                                dd[ index - 1 - r ] = "";
                            }
                        }
                    }

                    data.data.Add( dd );
                }

                foreach ( IRow row in sheet )
                {
                    if ( row.Cells.Count > 0 &&
                        row.Cells[ 0 ].ToString() == "End" )
                    {
                        break;
                    }

                    DataRow dataRow = data.dataTable.NewRow();

                    for ( int i = 0 ; i < row.Cells.Count ; i++ )
                    {
                        ICell cell = row.Cells[ i ];

                        if ( cell.ColumnIndex < data.dataTable.Columns.Count )
                        {
                            TableCell dataTableCell = new TableCell();
                            dataTableCell.style = cell.CellStyle;
                            dataTableCell.value = cell.ToString();

                            dataRow.SetField( cell.ColumnIndex , dataTableCell );
                        }
                    }

                    data.dataTable.Rows.Add( dataRow );
                }

                Log( "sheet " + sheet.SheetName );
                Log( "column count = " + columnCount );
                Log( "data count = " + data.data.Count );
            }

            workbook.Close();
        }

        static public void LoadExcel( string path )
        {
            FileStream stream = new FileStream( path , FileMode.Open , FileAccess.Read , FileShare.ReadWrite );

            Log( "load excel " + path );

            IWorkbook workbook = new XSSFWorkbook( stream );

            LoadExcel( workbook );

            Log( "excel loaded " + path );

            stream.Close();
            stream.Dispose();
        }


        static public void CheckData()
        {
            Dictionary<string , Dictionary<string , int>> dic = new Dictionary<string , Dictionary<string , int>>();
            for ( int i = 0 ; i < dataList.Count ; i++ )
            {
                TableData data = dataList[ i ];

                if ( data.headIndex == -1 )
                    LogError( "head index error " + " data " + data.name );
                if ( data.formatIndex == -1 )
                    LogError( "format index error " + " data " + data.name );
                if ( data.typeIndex == -1 )
                    LogError( "type index error " + " data " + data.name );
                if ( data.keyIndex == -1 )
                    LogError( "key index error " + " data " + data.name );

                if ( dic.ContainsKey( data.primaryKey ) )
                {
                    LogError( "primaryKey repeat " + " data " + data.name + " primaryKey " + data.primaryKey );
                    continue;
                }

                Dictionary<string , int> dic1 = new Dictionary<string , int>();
                dic.Add( data.primaryKey , dic1 );

                for ( int j = data.keyIndex + 1 ; j < data.data.Count ; j++ )
                {
                    string[] dt = data.data[ j ];
                    string key = dt[ data.primaryKeyIndex ];

                    if ( string.IsNullOrEmpty( key ) )
                    {
                        LogError( "key empty " + " data " + data.name + " row " + ( j + data.keyIndex ) );
                        continue;
                    }

                    Match mInfo = Regex.Match( key , @"^[A-Za-z0-9_-]+$" );

                    if ( !mInfo.Success )
                    {
                        LogError( "key must english and numbers " + " data " + data.name + " key '" + key + "'" );
                        continue;
                    }

                    if ( dic1.ContainsKey( key ) )
                    {
                        LogError( "key repeat " + " data " + data.name + " key '" + key + "'" );
                        continue;
                    }

                    dic1.Add( key , 1 );
                }
            }

            for ( int i = 0 ; i < dataList.Count ; i++ )
            {
                TableData data = dataList[ i ];

                for ( int j = 0 ; j < data.data[ data.headIndex ].Length ; j++ )
                {
                    string keyValue = data.data[ data.headIndex ][ j ];

                    if ( keyValue.Contains( "Key," ) )
                    {
                        string value = Regex.Split( keyValue , "Key," )[ 1 ];

                        Dictionary<string , int> dic1 = null;
                        if ( dic.TryGetValue( value , out dic1 ) )
                        {
                            string key = data.data[ data.keyIndex ][ j ];
                            if ( !dic1.ContainsKey( key ) )
                                LogError( "key not found " + " data " + data.name + " key '" + key + "'" );
                        }
                        else
                        {
                            LogError( "key not found " + " data " + data.name + " primaryKey '" + value + "'" );
                        }
                    }
                    else if ( keyValue.Contains( "Value," ) )
                    {
                        string value = Regex.Split( keyValue , "Value," )[ 1 ];
                        string format = data.data[ data.formatIndex ][ j ];

                        if ( !string.IsNullOrEmpty( format ) )
                        {
                            bool valueStart = false;
                            bool splitStart = false;
                            string splitTemp = "";
                            string valueTemp = "";
                            bool valueCheck = false;
                            List<string> splitList = new List<string>();
                            List<string> valueList = new List<string>();

                            for ( int s = 0 ; s < format.Length ; s++ )
                            {
                                char ss = format[ s ];
                                if ( ss == ')' )
                                {
                                    splitStart = true;

                                    if ( valueStart )
                                    {
                                        if ( valueTemp == value )
                                            valueCheck = true;

                                        valueStart = false;
                                        valueList.Add( valueTemp );
                                        valueTemp = "";
                                    }
                                    continue;
                                }
                                if ( ss == '(' )
                                {
                                    if ( splitStart )
                                    {
                                        splitStart = false;
                                        splitList.Add( splitTemp );
                                        splitTemp = "";
                                    }

                                    valueStart = true;
                                    continue;
                                }

                                if ( splitStart )
                                {
                                    splitTemp += ss;
                                }
                                if ( valueStart )
                                {
                                    valueTemp += ss;
                                }
                            }

                            splitList.Add( splitTemp );

                            if ( !valueCheck )
                            {
                                LogError( "key not found " + " data " + data.name + " key '" + value + "'" );
                                continue;
                            }

                            if ( splitList.Count != valueList.Count )
                            {
                                LogError( "format error " + " data " + data.name + " format '" + format + "'" );
                                continue;
                            }

                            Dictionary<string , int> dic1 = null;
                            if ( dic.TryGetValue( value , out dic1 ) )
                            {
                                for ( int k = data.keyIndex + 1 ; k < data.data.Count ; k++ )
                                {
                                    string key = data.data[ k ][ j ];
                                    string keyTemp = key;

                                    if ( splitList.Count == 1 )
                                    {
                                        string[] keys = Regex.Split( key , splitList[ 0 ] );

                                        for ( int s = 0 ; s < keys.Length ; s++ )
                                        {
                                            if ( valueList[ 0 ] != value )
                                            {
                                                continue;
                                            }

                                            if ( !string.IsNullOrEmpty( keys[ s ] ) )
                                            {
                                                if ( dic1.ContainsKey( keys[ s ] ) )
                                                {
                                                    continue;
                                                }
                                                else
                                                {
                                                    LogError( "key not found " + " data " + data.name + " primaryKey '" + value + "' key '" + keys[ s ] + "'" );
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        string[] key1 = Regex.Split( key , splitList[ splitList.Count - 1 ] );

                                        for ( int s2 = 0 ; s2 < key1.Length ; s2++ )
                                        {
                                            if ( !string.IsNullOrEmpty( key1[ s2 ] ) )
                                            {
                                                for ( int s1 = 0 ; s1 < splitList.Count - 1 ; s1++ )
                                                {
                                                    string[] keys = Regex.Split( key1[ s2 ] , splitList[ s1 ] );

                                                    for ( int s = 0 ; s < keys.Length ; s++ )
                                                    {
                                                        if ( valueList[ s ] != value )
                                                        {
                                                            continue;
                                                        }

                                                        if ( !string.IsNullOrEmpty( keys[ s ] ) &&
                                                            dic1.ContainsKey( keys[ s ] ) )
                                                        {
                                                            continue;
                                                        }
                                                        else
                                                        {
                                                            LogError( "key not found " + " data " + data.name + " primaryKey '" + value + "' key '" + keys[ s ] + "'" );
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }


                                }
                            }
                            else
                            {
                                LogError( "key not found " + " data " + data.name + " key '" + value + "'" );
                            }
                        }
                        else
                        {
                            Dictionary<string , int> dic1 = null;
                            if ( dic.TryGetValue( value , out dic1 ) )
                            {
                                for ( int k = data.keyIndex + 1 ; k < data.data.Count ; k++ )
                                {
                                    string key = data.data[ k ][ j ];

                                    if ( string.IsNullOrEmpty( key ) )
                                    {
                                        continue;
                                    }

                                    if ( dic1.ContainsKey( key ) )
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        LogError( "key not found " + " data " + data.name + " primaryKey '" + value + "' key '" + key + "'" );
                                    }
                                }
                            }
                            else
                            {
                                LogError( "key not found " + " data " + data.name + " key '" + value + "'" );
                            }
                        }

                    }

                }
            }

        }

        static public void LoadSaveExcelAll( string path )
        {
            string[] files = Directory.GetFiles( path ,
                "*.xlsx" ,
                SearchOption.AllDirectories );

            for ( int i = 0 ; i < files.Length ; i++ )
            {
                if ( files[ i ].Contains( "~" ) )
                    continue;
                if ( files[ i ].Contains( "$" ) )
                    continue;

                try
                {
                    LoadExcel( files[ i ] );
                }
                catch ( Exception e )
                {
                    LogError( "load excel error. " + files[ i ] );
                    LogError( e.Message );
                }
            }

            for ( int i = 0 ; i < dataList.Count ; i++ )
            {
                TableData data = dataList[ i ];
                SaveData( path , data );
            }

            CheckData();

            Log( "" );
            Log( "export config error count = " + errorCount );
            Log( "export config over." );

            File.WriteAllText( path + "log.txt" , log );

            Clear();

        }
    }


}

