#if UNITY_ANDROID
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using unicicd.Editor;

namespace unicicd.Editor.Build
{
    public class AndroidBuildWindowGUI : BuildWindowGUIBase
    {
        public override string PlatformName => "Android";
    }
}
#endif