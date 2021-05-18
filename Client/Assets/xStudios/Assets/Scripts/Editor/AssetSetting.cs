using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace X
{
    [CreateAssetMenu()]
    public class AssetSetting : ScriptableObject
    {
        public UnityEditor.BuildTarget buildTarget = UnityEditor.BuildTarget.StandaloneWindows;

        public List<string> configPathList = new List<string>();
    }

}

