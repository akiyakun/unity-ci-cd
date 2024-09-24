#if UNITY_IOS
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using unicicd.Editor;

namespace unicicd.Editor.Build
{
    public class iOSBuildWindowGUI : BuildWindowGUIBase
    {
        public override string PlatformName => "iOS";
    }
}
#endif