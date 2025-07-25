#if UNITY_IOS
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityCICD.Editor;

namespace UnityCICD.Editor
{
    public class iOSBuildWindowGUI : BuildWindowGUIBase
    {
        public override string PlatformName => "iOS";
    }
}
#endif