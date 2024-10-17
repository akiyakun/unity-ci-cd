using System.Collections.Generic;
// using UnityEditor;

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

    public class CICDOptionData
    {
        public int Int;
        public string String;
    }

    public class CICDBuildOptions
    {
        public CICDBuildMode BuildMode = CICDBuildMode.Current;
        public UnityEditor.BuildTarget BuildTarget = UnityEditor.BuildTarget.NoTarget;
        // public List<string> Scenes = new List<string>();
        // public List<string> OptionStrings = new List<string>();
        // public CICDBuildDirectory BuildDirectory = CICDBuildDirectory.Auto;

        // ValueのCICDOptionDataはnull許容です
        public Dictionary<string, CICDOptionData> Options = new Dictionary<string, CICDOptionData>();

        public bool CleanupBuildDirectory = true;

        // UnityEditor.BuildOptions.Development
        public bool UnityDevelopmentBuild = false;
        public bool WaitForManagedDebugger = false;


        public bool InAppDebug = false;

        // MEMO:
        // Windowsビルドだけどビューワー用ビルドをしたいときなどに
        public string JobName = "";

        // return false is cancel build.
        public System.Func<CICDBuilder, UnityEditor.BuildPlayerOptions, bool> OnBeforeBuildProcess;

        public System.Action<CICDBuilder, UnityEditor.BuildPlayerOptions> OnAfterBuildProcess;

        // デフォルト設定でセットアップ
        public void SetupDefaultSettings()
        {
            ApplyCurrentBuildTarget();
            // ApplyCurrentBuildSettingsScene();
        }

        public void ApplyCurrentBuildTarget()
        {
            BuildTarget = UnityEditor.EditorUserBuildSettings.activeBuildTarget;
            // buildTarget = EditorUserBuildSettings.selectedStandaloneTarget;
        }

        // public void ApplyCurrentBuildSettingsScene()
        // {
        //     Scenes = BuildUtility.GetBuildSettingsScene();
        // }

        public bool HasOption(string key)
        {
            return Options.ContainsKey(key);
        }

    }
}
