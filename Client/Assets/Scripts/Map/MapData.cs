using UnityEngine;
using System.Collections;
using X;
using System.Collections.Generic;
using XLua;

namespace X
{
    [LuaCallCSharp]
    public enum MapObjectType
    {
        InternalCube = 0,
        Cube,
        Plant,
        PlayerPosition,

        Count
    }


    [LuaCallCSharp]
    public class MapData
    {
        public string Name = "";
        public string Des = "";

        public short Width = 32;
        public short Height = 32;
        public short MaxPlayer = 4;
        public short MaxWatcher = 8;
    }

    [LuaCallCSharp]
    public class MapInfo
    {
        public string Hash = "";
        public string File = "";
        public string Path = "";

        public MapData MapData = new MapData();

    }

}
