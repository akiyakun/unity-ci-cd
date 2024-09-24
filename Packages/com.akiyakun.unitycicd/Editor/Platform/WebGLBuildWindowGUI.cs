#if UNITY_WEBGL
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using unicicd.Editor;

namespace unicicd.Editor.Build
{
    public class WebGLBuildWindowGUI : BuildWindowGUIBase
    {
        public override string PlatformName => "WebGL";
    }
}
#endif