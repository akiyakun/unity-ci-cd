#if UNITY_SWITCH
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using unicicd.Editor;

namespace unicicd.Editor
{
    public class SwitchBuildWindowGUI : BuildWindowGUIBase
    {
        public override string PlatformName => "Switch";
    }
}
#endif