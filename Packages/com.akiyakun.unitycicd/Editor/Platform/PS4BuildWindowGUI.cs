﻿#if UNITY_PS4
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using unicicd.Editor;

namespace unicicd.Editor
{
    public class PS4BuildWindowGUI : BuildWindowGUIBase
    {
        public override string PlatformName => "PS4";
    }
}
#endif