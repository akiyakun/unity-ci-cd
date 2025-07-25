#if UNITY_STANDALONE_OSX
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityCICD.Editor;

namespace UnityCICD.Editor
{
    public class macOSBuildWindowGUI : BuildWindowGUIBase
    {
        public override string PlatformName => "macOS";
    }
}
#endif