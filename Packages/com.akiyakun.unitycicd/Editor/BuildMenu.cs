#if __USE_UNICICD_BUILDMENU__
using UnityEngine;
using UnityEditor;

namespace unicicd.Editor
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
        public const string BuildsToolsMenu = BuildsMenu + "Tools/";

        public const int PriorityBuilds = 1000000;
        public const int PriorityTests  = 1000100;
        public const int PriorityBottom = 1001000;

        #region Builds
#if UNITY_STANDALONE_WIN
        [MenuItem(BuildsMenu + "Windows Build...", false, PriorityBuilds + 1)]
        static void Windows_Build() => PlatformBuildFactory.GetWindow().Initialize(CICDBuildMode.Current);
        [MenuItem(BuildsMenu + "Windows Build [Debug Preset]", false, PriorityBuilds + 2)]
        static void Windows_DebugBuild() => PlatformBuildFactory.GetWindow().Initialize(CICDBuildMode.Debug);
        [MenuItem(BuildsMenu + "Windows Build [Release Preset]", false, PriorityBuilds + 3)]
        static void Windows_ReleaseBuild() => PlatformBuildFactory.GetWindow().Initialize(CICDBuildMode.Release);
        [MenuItem(BuildsMenu + "Windows Build [Publish Preset]", false, PriorityBuilds + 4)]
        static void Windows_PublishBuild() => PlatformBuildFactory.GetWindow().Initialize(CICDBuildMode.Publish);
#endif

#if UNITY_STANDALONE_OSX
        [MenuItem(BuildsMenu + "macOS Build...", false, PriorityBuilds + 1)]
        static void macOS_Build() => PlatformBuildFactory.GetWindow().Initialize(CICDBuildMode.Current);
        [MenuItem(BuildsMenu + "macOS Build [Debug Preset]", false, PriorityBuilds + 2)]
        static void macOS_DebugBuild() => PlatformBuildFactory.GetWindow().Initialize(CICDBuildMode.Debug);
        [MenuItem(BuildsMenu + "macOS Build [Release Preset]", false, PriorityBuilds + 3)]
        static void macOS_ReleaseBuild() => PlatformBuildFactory.GetWindow().Initialize(CICDBuildMode.Release);
        [MenuItem(BuildsMenu + "macOS Build [Publish Preset]", false, PriorityBuilds + 4)]
        static void macOS_PublishBuild() => PlatformBuildFactory.GetWindow().Initialize(CICDBuildMode.Publish);
#endif

#if UNITY_WEBGL
        [MenuItem(BuildsMenu + "WebGL Build...", false, PriorityBuilds + 1)]
        static void WebGL_Build() => PlatformBuildFactory.GetWindow().Initialize(CICDBuildMode.Current);
        [MenuItem(BuildsMenu + "WebGL Build [Debug Preset]", false, PriorityBuilds + 2)]
        static void WebGL_DebugBuild() => PlatformBuildFactory.GetWindow().Initialize(CICDBuildMode.Debug);
        [MenuItem(BuildsMenu + "WebGL Build [Release Preset]", false, PriorityBuilds + 3)]
        static void WebGL_ReleaseBuild() => PlatformBuildFactory.GetWindow().Initialize(CICDBuildMode.Release);
        [MenuItem(BuildsMenu + "WebGL Build [Publish Preset]", false, PriorityBuilds + 4)]
        static void WebGL_PublishBuild() => PlatformBuildFactory.GetWindow().Initialize(CICDBuildMode.Publish);
#endif

#if UNITY_ANDROID
        [MenuItem(BuildsMenu + "Android Build...", false, PriorityBuilds + 1)]
        static void Android_Build() => PlatformBuildFactory.GetWindow().Initialize(CICDBuildMode.Current);
        [MenuItem(BuildsMenu + "Android Build [Debug Preset]", false, PriorityBuilds + 2)]
        static void Android_DebugBuild() => PlatformBuildFactory.GetWindow().Initialize(CICDBuildMode.Debug);
        [MenuItem(BuildsMenu + "Android Build [Release Preset]", false, PriorityBuilds + 3)]
        static void Android_ReleaseBuild() => PlatformBuildFactory.GetWindow().Initialize(CICDBuildMode.Release);
        [MenuItem(BuildsMenu + "Android Build [Publish Preset]", false, PriorityBuilds + 4)]
        static void Android_PublishBuild() => PlatformBuildFactory.GetWindow().Initialize(CICDBuildMode.Publish);
#endif

#if UNITY_IOS
        [MenuItem(BuildsMenu + "iOS Build...", false, PriorityBuilds + 1)]
        static void iOS_Build() => PlatformBuildFactory.GetWindow().Initialize(CICDBuildMode.Current);
        [MenuItem(BuildsMenu + "iOS Build [Debug Preset]", false, PriorityBuilds + 2)]
        static void iOS_DebugBuild() => PlatformBuildFactory.GetWindow().Initialize(CICDBuildMode.Debug);
        [MenuItem(BuildsMenu + "iOS Build [Release Preset]", false, PriorityBuilds + 3)]
        static void iOS_ReleaseBuild() => PlatformBuildFactory.GetWindow().Initialize(CICDBuildMode.Release);
        [MenuItem(BuildsMenu + "iOS Build [Publish Preset]", false, PriorityBuilds + 4)]
        static void iOS_PublishBuild() => PlatformBuildFactory.GetWindow().Initialize(CICDBuildMode.Publish);
#endif

#if UNITY_SWITCH
        [MenuItem(BuildsMenu + "Switch Build...", false, PriorityBuilds + 1)]
        static void Switch_Build() => PlatformBuildFactory.GetWindow().Initialize(CICDBuildMode.Current);
        [MenuItem(BuildsMenu + "Switch Build [Debug Preset]", false, PriorityBuilds + 2)]
        static void Switch_DebugBuild() => PlatformBuildFactory.GetWindow().Initialize(CICDBuildMode.Debug);
        [MenuItem(BuildsMenu + "Switch Build [Release Preset]", false, PriorityBuilds + 3)]
        static void Switch_ReleaseBuild() => PlatformBuildFactory.GetWindow().Initialize(CICDBuildMode.Release);
        [MenuItem(BuildsMenu + "Switch Build [Publish Preset]", false, PriorityBuilds + 4)]
        static void Switch_PublishBuild() => PlatformBuildFactory.GetWindow().Initialize(CICDBuildMode.Publish);
#endif

#if UNITY_PS4
        [MenuItem(BuildsMenu + "PS4 Build...", false, PriorityBuilds + 1)]
        static void PS4_Build() => PlatformBuildFactory.GetWindow().Initialize(CICDBuildMode.Current);
        [MenuItem(BuildsMenu + "PS4 Build [Debug Preset]", false, PriorityBuilds + 2)]
        static void PS4_DebugBuild() => PlatformBuildFactory.GetWindow().Initialize(CICDBuildMode.Debug);
        [MenuItem(BuildsMenu + "PS4 Build [Release Preset]", false, PriorityBuilds + 3)]
        static void PS4_ReleaseBuild() => PlatformBuildFactory.GetWindow().Initialize(CICDBuildMode.Release);
        [MenuItem(BuildsMenu + "PS4 Build [Publish Preset]", false, PriorityBuilds + 4)]
        static void PS4_PublishBuild() => PlatformBuildFactory.GetWindow().Initialize(CICDBuildMode.Publish);
#endif

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

        [MenuItem(BuildsMenu + "Open Build Directory...", false, PriorityBuilds + 601)]
        static void OpenBuildDirectory()
        {
            System.Diagnostics.Process.Start($"{BuildUtility.GetRootPath()}build");
        }

        [MenuItem(BuildsMenu + "Cleanup", false, PriorityBuilds + 602)]
        static void Build_Cleanup()
        {
            if (EditorUtility.DisplayDialog("確認", "ビルドフォルダを全て削除しますがよろしいですか？", "OK", "Cancel"))
            {
                CICDBuilder.CleanupAllBuildDirectory();
                UnityEngine.Debug.Log("Cleanup finished.");
            }
        }

        #endregion


        #region Builds/Tools

        [MenuItem(BuildsToolsMenu + "CommitInfoToClipboard", false, PriorityBuilds + 101)]
        static void GitCommitInfoToClipboard()
        {
            GUIUtility.systemCopyBuffer = Git.GetCommitInfo();
        }

        #endregion


        // FIXME: テストは別に専用メニューを用意するほうがよさそう
        // #region Tests
        // [MenuItem(BuildsMenu + "Tests Run...", false, PriorityTests + 0)]
        // static void Tests_Run()
        // {
        //     // SymbolEditor.AddSymbol("__TESTS__", BuildTargetGroup.Standalone);
        //     // SymbolEditor.AddSymbol("__TESTS__", BuildTargetGroup.Standalone);

        //     if (EditorUtility.DisplayDialog("確認", "Test Runner を実行しますがよろしいですか？", "OK", "Cancel"))
        //     {
        //         // SymbolEditor.AddSymbol("__TESTS__", BuildTargetGroup.Standalone);
        //         Debug.Log("OK");
        //     }
        //     else
        //     {
        //         Debug.Log("Cancel");
        //     }

        // }
        // #endregion

#if false
        // 作業用
        [MenuItem(BuildsMenu + "CommandLine Build", false, PriorityBottom)]
        static void CommandLineBuild()
        {
            CI.EditorDebug = true;
            CommandLineHelper.AddCommandLineArgs("--platform", "StandaloneWindows64");
            CommandLineHelper.AddCommandLineArgs("--buildmode", "Debug");
            CommandLineHelper.AddCommandLineArgs("--inappdebug");
            CI.Build();
        }
#endif

    }
}
#endif
