#if UNITY_STANDALONE_WIN
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using unicicd.Editor;

namespace unicicd.Editor.Build
{
    public class WindowsBuildWindowGUI : BuildWindowGUIBase
    {
        public override string PlatformName => "Windows";

        bool platformGroup = true;

        bool isMonoBuild;

        protected override void OnPlatformGUI()
        {
            platformGroup = EditorGUILayout.BeginFoldoutHeaderGroup(platformGroup, "Platform Options");
            if (platformGroup)
            {
                isMonoBuild = EditorGUILayout.Toggle(WindowsPlatformBuildOptions.MonoBuild, isMonoBuild);
                // EditorGUILayout.LabelField("開いてるよ");
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

        }

        protected override void OnBuildBegin(CICDBuildOptions buildOptions)
        {
            if (isMonoBuild)
            {
                buildOptions.OptionStrings.Add(WindowsPlatformBuildOptions.MonoBuild);
            }
        }

        // protected override void OnBuildEnd(CICDBuildOptions buildOptions)
        // {
        // }

        // protected override void OnLoadSettings()
        // {
        //     base.OnLoadSettings();
        // }

        // protected override void OnSaveSettings()
        // {
        //     base.OnSaveSettings();
        // }

    }
}
#endif