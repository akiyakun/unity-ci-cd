using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace unicicd.Editor
{
    /*
        Unityのビルドを処理するクラスです。

        コンソールで実行時にログが見たいので Log() メソッドでログの出力先を適宜変更しています。
    */
    public class CICDBuilder
    {
        public static readonly string BuildRootDirectory = "build";

        public bool Initialized { get; private set; }
        public string WorkingBuildDirectory { get; private set; }

        public CICDConfig Config { get; private set; }
        public CICDBuildOptions BuildOptions { get; private set; }

        public CICDBuilder()
        {
            Config = CICDConfig.Load();
            Debug.Assert(Config != null);
            Debug.Assert(Config.BuildSettings.PlatformBuildFactory != null);
        }

        private static void Log(string msg)
        {
#if UNITY_EDITOR
            Debug.Log(msg);
#else
            Console.WriteLine(msg);
#endif
        }

        private static void LogError(string msg)
        {
#if UNITY_EDITOR
            Debug.LogError(msg);
#else
            Console.SetError(msg);
#endif
        }

        public static void CleanupAllBuildDirectory()
        {
            BuildUtility.DeleteDirectory(BuildRootDirectory);
            BuildUtility.CreateDirectory(BuildRootDirectory);
        }

        // ビルドモードのルートビルドディレクトリを削除します
        public static void CleanupBuildModeRootDirectory(CICDBuildMode buildMode)
        {
            string path = GetWorkingBuildModeRootDirectory(buildMode);
            BuildUtility.DeleteDirectory(path);
        }

        // ビルドターゲットのビルドディレクトリを削除します
        public void CleanupBuildTargetDirectory()
        {
            Debug.Log("CleanupBuildDirectory: " + WorkingBuildDirectory);
            BuildUtility.DeleteDirectory(WorkingBuildDirectory);
        }

        // ビルドモード別のディレクトリパスを取得
        public static string GetWorkingBuildModeRootDirectory(CICDBuildMode buildMode)
        {
            switch (buildMode)
            {
                case CICDBuildMode.Current:
                    return $"{BuildRootDirectory}/current/";
                case CICDBuildMode.Debug:
                    return $"{BuildRootDirectory}/debug/";
                case CICDBuildMode.Release:
                    return $"{BuildRootDirectory}/release/";
                case CICDBuildMode.Publish:
                    return $"{BuildRootDirectory}/publish/";
            }

            return "";
        }

        // ワーキングビルドディレクトリパスを作成
        void CreateWorkingBuildDirectory(IPlatformBuild platformBuild)
        {
            WorkingBuildDirectory = BuildUtility.PathCombine(
                GetWorkingBuildModeRootDirectory(platformBuild.BuildOptions.BuildMode),
                platformBuild.GetBuildDirectoryName()
            );
        }

        IPlatformBuild platformBuild = null;

        public bool Initialize(CICDBuildOptions options)
        {
            Debug.Assert(options != null);
            BuildOptions = options;

            // platformBuild = Config.BuildSettings.PlatformBuildFactory.Create(options.BuildTarget.ToString());
            platformBuild = Config.BuildSettings.PlatformBuildFactory.Create();

// #if UNITY_STANDALONE_WIN
//             Debug.Log("[Build] WindowsPlatformBuild");
//             platformBuild = new WindowsPlatformBuild();
// #elif UNITY_STANDALONE_OSX
//             Debug.Log("[Build] macOSPlatformBuild");
//             platformBuild = new macOSPlatformBuild();
// #elif UNITY_WEBGL
//             Debug.Log("[Build] WebGLPlatformBuild");
//             platformBuild = new WebGLPlatformBuild();
// #elif UNITY_ANDROID
//             Debug.Log("[Build] AndroidPlatformBuild");
//             platformBuild = new AndroidPlatformBuild();
// #elif UNITY_IOS
//             Debug.Log("[Build] iOSPlatformBuild");
//             platformBuild = new iOSPlatformBuild();
// #elif UNITY_SWITCH
//             Debug.Log("[Build] SwitchPlatformBuild");
//             platformBuild = new SwitchPlatformBuild();
// #elif UNITY_PS4
//             Debug.Log("[Build] PS4PlatformBuild");
//             platformBuild = new PS4PlatformBuild();
// // #else
// // UnityEditor上だと UNITY_INCLUDE_TESTS がランタイムでも有効になっている
// // #if UNITY_INCLUDE_TESTS
//             platformBuild = new MockPlatformBuild();
// #endif

            if (platformBuild == null)
            {
                Debug.Assert(false, "Unknown platform.");
                return false;
            }

            if (platformBuild.Initialize(options) == false) return false;
            CreateWorkingBuildDirectory(platformBuild);

            Initialized = true;
            return true;
        }

        // ビルドを実行します
        public CICDBuildResult Build()
        {
            var result = new CICDBuildResult();
            if (Initialized == false) return result;

            // FIXME: CICDBuildMode.Currentのときのみ処理したい
            // if (BuildOptions.BuildTarget == BuildTarget.NoTarget)
            // {
            //     BuildOptions.ApplyCurrentBuildTarget();
            // }

            if (BuildOptions.CleanupBuildDirectory)
            {
                CleanupBuildTargetDirectory();
            }

            // 現在のDefineを一時保存
            var saveSymbols = SymbolEditor.GetSymbols();

            try
            {
                result = _Build();
            }
            finally
            {
                switch (BuildOptions.BuildMode)
                {
                    case CICDBuildMode.Current:
                        break;
                    default:
                        // Defineを元に戻す
                        SymbolEditor.SetSymbols(saveSymbols);
                        SymbolEditor.RemoveSymbol("__TESTS__");
                        SymbolEditor.RemoveSymbol("__PUBLISH__");
                        break;
                }
            }

            return result;
        }

        CICDBuildResult _Build()
        {
            var result = new CICDBuildResult();

            if (BuildOptions.InAppDebug == true)
            {
                SymbolEditor.AddSymbol("__INAPPDEBUG__");
            }
            else
            {
                SymbolEditor.RemoveSymbol("__INAPPDEBUG__");
            }

            switch (BuildOptions.BuildMode)
            {
                case CICDBuildMode.Current:
                    Log("[Build] Current Build.");
                    break;
                case CICDBuildMode.Debug:
                    Log("[Build] Debug Build.");
                    SymbolEditor.AddSymbol("__DEBUG__");
                    SymbolEditor.RemoveSymbol("__RELEASE__");
                    SymbolEditor.RemoveSymbol("__TESTS__");
                    SymbolEditor.RemoveSymbol("__PUBLISH__");
                    break;
                case CICDBuildMode.Release:
                    Log("[Build] Release Build.");
                    SymbolEditor.RemoveSymbol("__DEBUG__");
                    SymbolEditor.RemoveSymbol("__TESTS__");
                    SymbolEditor.RemoveSymbol("__PUBLISH__");
                    SymbolEditor.AddSymbol("__RELEASE__");
                    break;
                default:
                    return result;
            }

            BuildPlayerOptions bpo = new BuildPlayerOptions();

            // ビルドターゲットを変更
            if (!SwitchBuildTarget(BuildOptions.BuildTarget, ref bpo))
            {
                Log("[Build] SwitchBuildTarget() failed.");
                return result;
            }

            // ビルドプレイヤーオプションの設定
            {
                SettingBuildPlayerOptions(BuildOptions, ref bpo);

                result.BuildDirectory = WorkingBuildDirectory;

                // ロケーションパスの設定はココで…
                // bpo.locationPathName = CreateLocationPathName(
                //     Config, result.BuildDirectory, BuildOptions);
                bpo.locationPathName = BuildUtility.PathCombine(
                    WorkingBuildDirectory, platformBuild.CreateLocationPathName());

                // BuildPipeline.BuildPlayer()はフォルダが無いとエラーが出る
                Debug.Log("Location: " + bpo.locationPathName);
                BuildUtility.CreateDirectory(bpo.locationPathName);
            }

            // ビルド前処理
            {
                Log("[Build] Platform OnBeforeBuildProcess().");
                bool success = platformBuild.OnBeforeBuildProcess(this, bpo);

                if (success == true && BuildOptions.OnBeforeBuildProcess != null)
                {
                    Log("[Build] BuildOptions OnBeforeBuildProcess().");
                    success = BuildOptions.OnBeforeBuildProcess.Invoke(this, bpo);
                }

                // キャンセルの場合、ビルド後処理を実行する
                if (success == false)
                {
                    Log("[Build] Platform OnAfterBuildProcess().");
                    platformBuild.OnAfterBuildProcess(this, bpo);

                    Log("[Build] BuildOptions OnAfterBuildProcess().");
                    BuildOptions.OnAfterBuildProcess?.Invoke(this, bpo);

                    return result;
                }
            }

            // ビルドを実行
            BuildReport report = null;
            try
            {
                report = BuildPipeline.BuildPlayer(bpo);
                Log("[Build] Finished BuildPlayer().");

                // PostprocessBuild(bpo, buildOptions);
                // Log("[Build] Finished PostprocessBuild().");
            }
            catch (System.Exception e)
            {
                LogError("[Build] Exception:" + e.Message);
            }
            // finally
            // {
            // }

            Log(report.ToString());

            if (report.summary.result != BuildResult.Succeeded)
            {
                LogError("[Build] Build error.");
                result.BuildSucceeded = false;
            }
            else
            {
                Log("[Build] Build completed!");
                result.BuildSucceeded = true;
            }

            // ビルド後処理
            {
                Log("[Build] Platform OnAfterBuildProcess().");
                platformBuild.OnAfterBuildProcess(this, bpo);

                Log("[Build] BuildOptions OnAfterBuildProcess().");
                BuildOptions.OnAfterBuildProcess?.Invoke(this, bpo);
            }

            return result;
        }

        // ターゲット毎のビルドプレイヤーオプション設定
        public /*static*/ void SettingBuildPlayerOptions(CICDBuildOptions options, ref BuildPlayerOptions bpo)
        {
            // SwitchBuildTarget()のほうで設定される
            // bpo.target = options.BuildTarget;
            // bpo.targetGroup

            bpo.options = UnityEditor.BuildOptions.None;

            // if (options.Scenes.Count <= 0)
            // {
            //     bpo.scenes = BuildUtility.GetBuildSettingsScene().ToArray();
            //     Debug.Log(bpo.scenes[0]);
            // }
            // else
            // {
            //     bpo.scenes = options.Scenes.ToArray();
            // }
            {
                List<string> scenes = new List<string>();

                // 必須シーンをリストに追加
                scenes.AddRange(Config.BuildSettings.GetRequiredScenePathArray());

                // InAppDebugシーンをリストに追加
                if (BuildOptions.InAppDebug == true)
                {
                    scenes.AddRange(Config.BuildSettings.GetInAppDebugScenePathArray());
                }

                bpo.scenes = scenes.ToArray();
            }

        }

        /*
            ロケーションパス名を取得
            ビルドターゲットによってフォルダのみの指定であったり、拡張子付きのファイルパスであったり異なる指定のため。
         */
        public static string CreateLocationPathName(IPlatformBuild platformBuild)
        {
            var config = CICDConfig.Load();
            CICDBuildOptions options = platformBuild.BuildOptions;
            string app_name = "";

            // JobNameが空の場合はプラットフォーム名をそのまま使う
            if (string.IsNullOrEmpty(options.JobName))
            {
                var job = config.jobs.Find(obj => obj.platform == platformBuild.PlatformName);
                if (job != null)
                {
                    app_name = job.application_filename;
                }
            }
            else
            // JobNameが指定されている場合
            {
                Log("jobName = " + options.JobName);
                var job = config.jobs.Find(obj => obj.platform == platformBuild.PlatformName && obj.job_name == options.JobName);
                if (job == null)
                {
                    Log("[Build] No matching Job was found.");
                    Debug.Assert(false);
                }
                else
                {
                    app_name = job.application_filename;
                }
            }

            string ret = "";
            if (string.IsNullOrEmpty(app_name))
            {
                // アプリファイル名の設定なし
                // string mode = options.BuildMode.ToString();

// #if (UNITY_EDITOR && UNITY_ANDROID)
//                 if (PreprocessorBuild.AndroidMonoBuild) mode += "(Mono)";
// #endif
                // MEMO: ビルドの度にファイル名が変わるのは自動化観点からも好ましくない
                // string info = string.Format(" - {0} v{1}({2})", mode, Application.version, BuildUtility.GetBuildNumber());
                // ret = string.Format("{0}{1}{2}", Application.productName, info, platformBuild.ExtensionName);

                ret = $"{Application.productName}{platformBuild.ExtensionName}";
            }
            // else
            // {
            //     // アプリファイル名が設定されている
            //     ret = string.Format("{0}{1}", app_name, platformBuild.ExtensionName);
            // }
            else
            {
                ret = $"{app_name}{platformBuild.ExtensionName}";
            }

            // return BuildUtility.PathCombine(platformBuild.GetBuildDirectoryName(), ret);
            return ret;
        }

#if false
        /*  ロケーションパス名を取得
            実行ファイルを直接ビルドするモードの場合、拡張子付きのファイルパス等を含める必要があるため。

            MEMO: platform名の文字列を直接書いているのでどうにかしたい
         */
        public static string CreateLocationPathName(
            CICDConfig config, string workPath, CICDBuildOptions options)
        {
            string app_name = "";
            string extension = "";
            string platform = "";

            switch (options.BuildTarget)
            {
                case BuildTarget.Android:
                    extension = ".apk";
                    platform = "Android";
                    break;
                case BuildTarget.iOS:
                    platform = "iOS";
                    // return BuildUtility.PathCombine(workPath, ret);
                    return workPath;
                    // break;
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    extension = ".exe";
                    platform = "Windows";
                    break;
                case BuildTarget.StandaloneOSX:
                    extension = ".app";
                    platform = "macOS";
                    break;
                case BuildTarget.WebGL:
                    break;
                case BuildTarget.Switch:
                    platform = "Switch";
                    break;
                case BuildTarget.PS4:
                    platform = "PS4";
                    break;
                default:
                    Log("[Build] Unknown BuildTarget is " + options.BuildTarget.ToString());
                    Debug.Assert(false);
                    break;
            }

            string ret = "";

            if (string.IsNullOrEmpty(options.JobName))
            {
                var job = config.jobs.Find(obj => obj.platform == platform);
                if (job != null)
                {
                    app_name = job.application_filename;
                }
            }
            else
            {
                Log("jobName = " + options.JobName);
                var job = config.jobs.Find(obj => obj.platform == platform && obj.job_name == options.JobName);
                if (job == null)
                {
                    Log("[Build] No matching Job was found.");
                    Debug.Assert(false);
                }
                else
                {
                    app_name = job.application_filename;
                }
            }

            if (app_name.Length <= 0)
            {
                // アプリファイル名の設定なし
                string mode = options.BuildMode.ToString();

#if (UNITY_EDITOR && UNITY_ANDROID)
            if (PreprocessorBuild.AndroidMonoBuild) mode += "(Mono)";
#endif
                string info = string.Format(" - {0} v{1}({2})", mode, Application.version, BuildUtility.GetBuildNumber());
                ret = string.Format("{0}{1}{2}", Application.productName, info, extension);
            }
            else
            {
                // アプリファイル名が設定されている
                ret = string.Format("{0}{1}", app_name, extension);
            }

            return BuildUtility.PathCombine(workPath, ret);
        }
#endif

        // PreprocessorBuild.csで処理するように変更されました
        /*
        // ターゲット毎のデフォルトオプション設定
        public static void SetDefaultTargetOptions(BuildTarget buildTarget)
        {
            switch (buildTarget)
            {
                case BuildTarget.Android:
                    Console.WriteLine("[Build] Set IL2CPP");
                    PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
                    // PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Android, ApiCompatibilityLevel.NET_4_6);
                    break;
                case BuildTarget.iOS:
                    Console.WriteLine("[Build] Set IL2CPP");
                    PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS, ScriptingImplementation.IL2CPP);

                    PlayerSettings.iOS.appleEnableAutomaticSigning = true;
                    break;
                case BuildTarget.StandaloneWindows:
                    Console.WriteLine("[Build] Set Mono2x");
                    PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.Mono2x);
                    break;
                case BuildTarget.StandaloneWindows64:
                    Console.WriteLine("[Build] Set Mono2x");
                    PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.Mono2x);
                    break;
                case BuildTarget.StandaloneOSX:
                    Console.WriteLine("[Build] Set Mono2x");
                    PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.Mono2x);
                    // PlayerSettings.SetArchitecture(BuildTargetGroup.Standalone, 1);     // 64bit
                    break;
                default:
                    Console.WriteLine("[Build] Unknown BuildTarget is " + buildTarget.ToString());
                    Debug.Assert(false);
                    break;
            }
        }
        */

        /*
            BuildTargetGroupとBuildTargetの二つのパラメータを取る為に用意
         */
        public static bool SwitchBuildTarget(BuildTarget buildTarget, ref BuildPlayerOptions bpo)
        {
            BuildTargetGroup group = BuildTargetGroup.Unknown;

            switch (buildTarget)
            {
                case BuildTarget.Android:
                    group = BuildTargetGroup.Android;
                    break;
                case BuildTarget.iOS:
                    group = BuildTargetGroup.iOS;
                    break;
                case BuildTarget.StandaloneWindows:
                    group = BuildTargetGroup.Standalone;
                    break;
                case BuildTarget.StandaloneWindows64:
                    group = BuildTargetGroup.Standalone;
                    break;
                case BuildTarget.StandaloneOSX:
                    group = BuildTargetGroup.Standalone;
                    break;
                case BuildTarget.WebGL:
                    group = BuildTargetGroup.WebGL;
                    break;
                case BuildTarget.Switch:
                    group = BuildTargetGroup.Switch;
                    break;
                case BuildTarget.PS4:
                    group = BuildTargetGroup.PS4;
                    break;
                default:
                    Log("[Build] Unknown BuildTarget is " + buildTarget.ToString());
                    Debug.Assert(false);
                    break;
            }

            bpo.target = buildTarget;
            bpo.targetGroup = group;

            return EditorUserBuildSettings.SwitchActiveBuildTarget(group, buildTarget);
        }

    }
}
