#if UNITY_WEBGL
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityCICD.Editor;

namespace UnityCICD.Editor
{
    public class WebGLBuildWindowGUI : BuildWindowGUIBase
    {
        public override string PlatformName => "WebGL";
    }
}
#endif