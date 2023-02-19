#if __USE_UNICICD_BUILDMENU__
using UnityEditor;

namespace unicicd.Editor.Build
{
    public class BuildMenu
    {
        public enum BuildMode
        {
            Debug,
            Release,
            AndroidMono,
        }

        public const string TopMenu = "App/";
        public const string BuildsMenu = TopMenu + "Builds/";

        public const int PriorityBuilds = 1000;
        public const int PriorityTests = 2000;

        #region Builds
        [MenuItem(BuildsMenu + "Debug Build", false, PriorityBuilds + 0)]
        static void DebugBuild()
        {
            var window = EditorWindow.GetWindow<BuildWindow>(true);
            window.Initialize(BuildMode.Debug);
        }

        [MenuItem(BuildsMenu + "Release Build", false, PriorityBuilds + 1)]
        static void ReleaseBuild()
        {
            var window = EditorWindow.GetWindow<BuildWindow>(true);
            window.Initialize(BuildMode.Release);
        }

        /*
        #if (UNITY_EDITOR && UNITY_ANDROID)
                [MenuItem(BuildsMenu + "Android Mono Build", false, PriorityBuilds + 80)]
                static void AndroidMonoBuild()
                {
                    var window = EditorWindow.GetWindow<BuildWindow>(true);
                    window.Initialize(BuildMode.AndroidMono);
                }

                [MenuItem(BuildsMenu + "Install (*.apk)", false, PriorityBuilds + 81)]
                public static void AndroidInstallAPK()
                {
                    var config = CICDConfig.Load();

                    string path = CICDBuilder.CreateLocationPathName(config,
                        CICDBuilder.GetWorkingBuildDirectory(true, BuildTarget.Android), BuildTarget.Android, true, "");
                    path = path.Replace(" ", "\\ ").Replace("(", "\\(").Replace(")", "\\)");

                    string cmd = string.Format("adb install -r \"\"{0}\"\"", path);

                    // string workDir = "/Users/akiya/local/opt/android-sdk/platform-tools";
                    string workDir = BuildUtility.GetRootPath();
                    if (0 == BuildUtility.DoConsoleCommand(cmd, workDir))
                    {
                        // FIXMW: idを外からちゃんと渡す
                        BuildUtility.DoConsoleCommand("adb shell am start -n com.akiyakun.prototype/com.unity3d.player.UnityPlayerActivity", workDir);
                    }

                    Debug.Log("AndroidInstallAPK Finish.");
                }
        #endif
        */

// #if (UNITY_EDITOR && UNITY_IOS)
//         [MenuItem(BuildsMenu + "Xcode build (*.ipa) [debug]", false, PriorityBuilds + 81)]
//         static void build_ipa()
//         {
//             bool isDebug = true;
//             string strMode = isDebug ? "debug" : "release";

//             var builder = new CICDBuilder();
//             builder.Build(isDebug, "", EditorUserBuildSettings.activeBuildTarget);

//             string workDir = BuildUtility.GetRootPath() + "tools/build/";
//             string proj_dir = string.Format("../../build/{0}/iOS/xcode/", strMode);

//             BuildUtility.DoConsoleCommand(string.Format(
//                 "python3 build_ipa.py -proj_dir \"{1}\" -archive_path '../../build/{0}/iOS/app.xcarchive' -ipa_plist 'ipa_adhoc.plist' -out_ipa_dir '../../build/{0}/iOS/ipa/' > ../../build/{0}_ios_ipa.log",
//                 strMode, proj_dir), workDir);
//         }
// #endif

        [MenuItem(BuildsMenu + "Cleanup", false, PriorityBuilds + 990)]
        static void Build_Cleanup()
        {
            CICDBuilder.CleanupBuildDirectory(true);
            CICDBuilder.CleanupBuildDirectory(false);
            UnityEngine.Debug.Log("Cleanup finished.");
        }

        #endregion

        #region Tests
        [MenuItem(BuildsMenu + "Tests Run...", false, PriorityTests + 0)]
        static void Tests_Run()
        {
            // SymbolEditor.AddSymbol("__TESTS__", BuildTargetGroup.Standalone);
            SymbolEditor.AddSymbol("__TESTS__", BuildTargetGroup.Standalone);

        }
        #endregion
    }
}
#endif
