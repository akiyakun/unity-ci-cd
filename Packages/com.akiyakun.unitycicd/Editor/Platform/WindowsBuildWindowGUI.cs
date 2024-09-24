#if UNITY_STANDALONE_WIN
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using unicicd.Editor;

namespace unicicd.Editor.Build
{
    public class WindowsBuildWindowGUI : BuildWindowGUIBase
    {
        public override string PlatformName => "Windows";
    }
}
#endif