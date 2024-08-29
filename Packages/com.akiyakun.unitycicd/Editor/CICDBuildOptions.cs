using System.Collections.Generic;
using UnityEditor;

namespace unicicd.Editor.Build
{
    // public enum CICDBuildDirectory
    // {
    //     Auto,
    // }

    public enum CICDBuildMode
    {
        Current,       // 現在の設定のままビルド
        Debug,
        Release,
        Publish,
    }

    public class CICDBuildOptions
    {
        public CICDBuildMode BuildMode = CICDBuildMode.Current;
        public string JobName = "";
        public BuildTarget BuildTarget = BuildTarget.NoTarget;
        // public List<string> Scenes = new List<string>();
        public List<string> OptionStrings = new List<string>();
        // public CICDBuildDirectory BuildDirectory = CICDBuildDirectory.Auto;


        public bool CleanupBuildDirectory = true;

        // UnityEditor.BuildOptions.Development
        public bool UnityDevelopmentBuild = false;
        public bool WaitForManagedDebugger = false;

        public Dictionary<string, int> PlatformIntOptions = new Dictionary<string, int>();
        public Dictionary<string, string> PlatformStringOptions = new Dictionary<string, string>();

        public bool InAppDebug = false;

        // return false is cancel build.
        public System.Func<CICDBuilder, BuildPlayerOptions, bool> OnBeforeBuildProcess;

        public System.Action<CICDBuilder> OnAfterBuildProcess;

        // デフォルト設定でセットアップ
        public void SetupDefaultSettings()
        {
            ApplyCurrentBuildTarget();
            // ApplyCurrentBuildSettingsScene();
        }

        public void ApplyCurrentBuildTarget()
        {
            BuildTarget = EditorUserBuildSettings.activeBuildTarget;
            // buildTarget = EditorUserBuildSettings.selectedStandaloneTarget;
        }

        // public void ApplyCurrentBuildSettingsScene()
        // {
        //     Scenes = BuildUtility.GetBuildSettingsScene();
        // }
    }
}
