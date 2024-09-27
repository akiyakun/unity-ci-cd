#if UNITY_WEBGL
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using unicicd.Editor;

namespace unicicd.Editor
{
    public class WebGLBuildWindowGUI : BuildWindowGUIBase
    {
        public override string PlatformName => "WebGL";
    }
}
#endif