using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace unicicd.Editor.Build
{
    public class CICDBuildOptions
    {
        public enum BuildMode
        {
            Current,       // 現在の設定のままビルド
            Debug,
            Release,
        }

        public BuildMode Build = BuildMode.Current;
        public string JobName = "";
        public BuildTarget BuildTarget = BuildTarget.NoTarget;
        public List<string> Scenes = new List<string>();
        public List<string> OptionStrings = new List<string>();


        public bool CleanupBuildDirectory = true;

        // UnityEditor.BuildOptions.Development
        public bool UnityDevelopmentBuild = false;


        // デフォルト設定でセットアップ
        public void SetupDefaultSettings()
        {
            ApplyCurrentBuildTarget();
            ApplyCurrentBuildSettingsScene();
        }

        public void ApplyCurrentBuildTarget()
        {
            BuildTarget = EditorUserBuildSettings.activeBuildTarget;
            // buildTarget = EditorUserBuildSettings.selectedStandaloneTarget;
        }

        public void ApplyCurrentBuildSettingsScene()
        {
            Scenes = BuildUtility.GetBuildSettingsScene();
        }
    }
}
