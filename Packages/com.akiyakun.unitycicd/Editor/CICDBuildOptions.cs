using System.Collections.Generic;
using UnityEditor;

namespace unicicd.Editor
{
    // public enum CICDBuildDirectory
    // {
    //     Auto,
    // }

    public enum CICDBuildMode
    {
        // 現在の設定のままビルド
        Current = 0,

        // デバッグビルド
        // __DEBUG__ が定義される
        // DevelopmentBuild フラグが強制ON
        Debug,

        // リリースビルド
        // __RELEASE__ が定義される
        Release,

        // パブリッシュビルド
        // __PUBLISH__ が定義される
        Publish,
    }

    public class CICDBuildOptions
    {
        public CICDBuildMode BuildMode = CICDBuildMode.Current;
        public BuildTarget BuildTarget = BuildTarget.NoTarget;
        // public List<string> Scenes = new List<string>();
        public List<string> OptionStrings = new List<string>();
        // public CICDBuildDirectory BuildDirectory = CICDBuildDirectory.Auto;


        public bool CleanupBuildDirectory = true;

        // UnityEditor.BuildOptions.Development
        public bool UnityDevelopmentBuild = false;
        public bool WaitForManagedDebugger = false;

        // public Dictionary<string, int> PlatformIntOptions = new Dictionary<string, int>();
        // public Dictionary<string, string> PlatformStringOptions = new Dictionary<string, string>();

        public bool InAppDebug = false;

        // MEMO:
        // Windowsビルドだけどビューワー用ビルドをしたいときなどに
        public string JobName = "";

        // return false is cancel build.
        public System.Func<CICDBuilder, BuildPlayerOptions, bool> OnBeforeBuildProcess;

        public System.Action<CICDBuilder, BuildPlayerOptions> OnAfterBuildProcess;

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

        public bool HasOption(string option)
        {
            return OptionStrings.Contains(option);
        }

    }
}
