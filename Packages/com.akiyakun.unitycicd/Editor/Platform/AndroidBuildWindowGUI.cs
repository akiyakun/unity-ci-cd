#if UNITY_ANDROID
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityCICD.Editor;

namespace UnityCICD.Editor
{
    public class AndroidBuildWindowGUI : BuildWindowGUIBase
    {
        public override string PlatformName => "Android";
    }
}
#endif