﻿#if UNITY_STANDALONE_OSX
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using unicicd.Editor;

namespace unicicd.Editor
{
    public class macOSBuildWindowGUI : BuildWindowGUIBase
    {
        public override string PlatformName => "macOS";
    }
}
#endif