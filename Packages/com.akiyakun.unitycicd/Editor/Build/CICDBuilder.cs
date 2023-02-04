using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

namespace unicicd.Editor.Build
{
    /*
        Unityのビルドを処理するクラスです。

		コンソールで実行時にログが見たいので Log() メソッドでログの出力先を適宜変更しています。
    */
    public class CICDBuilder
    {
        CICDConfig config;

        public CICDBuilder()
        {
            config = CICDConfig.Load();
        }

        private static void Log(string msg)
        {
#if UNITY_EDITOR
            Debug.Log(msg);
#else
            Console.WriteLine(msg);
#endif
        }

        // ターゲットのビルドディレクトリを削除します
        public static void CleanupBuildDirectory(bool development, BuildTarget buildTarget = BuildTarget.NoTarget)
        {
            // if (buildTarget == BuildTarget.NoTarget) buildTarget = EditorUserBuildSettings.selectedStandaloneTarget;
            if (buildTarget == BuildTarget.NoTarget) buildTarget = EditorUserBuildSettings.activeBuildTarget;
            string path = GetWorkingBuildDirectory(development, buildTarget);
            Debug.Log("CleanupBuildDirectory: " + path);
            BuildUtility.DeleteDirectory(path);
        }

        // ターゲットのビルドディレクトリを取得
        public static string GetWorkingBuildDirectory(bool development, BuildTarget buildTarget = BuildTarget.NoTarget)
        {
            // if (buildTarget == BuildTarget.NoTarget) buildTarget = EditorUserBuildSettings.selectedStandaloneTarget;
            if (buildTarget == BuildTarget.NoTarget) buildTarget = EditorUserBuildSettings.activeBuildTarget;

            string path = "build/";

            if (development) path += "debug/";
            else path += "release/";

            switch (buildTarget)
            {
                case BuildTarget.Android: path += "Android/"; break;
                case BuildTarget.iOS: path += "iOS/xcode"; break;
                case BuildTarget.StandaloneWindows: path += "Win32/"; break;
                case BuildTarget.StandaloneWindows64: path += "Win64/"; break;
                case BuildTarget.StandaloneOSX: path += "macOS/"; break;
                case BuildTarget.WebGL: path += "WebGL/"; break;
                default:
                    Log("[Build] Unknown BuildTarget is " + buildTarget.ToString());
                    Debug.Assert(false);
                    break;
            }

            return path;
        }

        // ビルドを実行します
        public CICDBuildResult Build(bool development, string jobName = "", string[] optionStrings = null, BuildTarget buildTarget = BuildTarget.NoTarget)
        {
            // if (buildTarget == BuildTarget.NoTarget) buildTarget = EditorUserBuildSettings.selectedStandaloneTarget;
            if (buildTarget == BuildTarget.NoTarget) buildTarget = EditorUserBuildSettings.activeBuildTarget;

            // MEMO: 既存ファイルがあるとビルドに支障が出る事が多々あるので削除を強制しています
            CleanupBuildDirectory(development, buildTarget);

            // 現在のDefineを一時保存
            var saveSymbols = SymbolEditor.GetSymbols();

            var result = new CICDBuildResult();
            try
            {
                result = _Build(development, jobName, optionStrings, buildTarget);
            }
            finally
            {
                // Defineを元に戻す
                SymbolEditor.SetSymbols(saveSymbols);
                SymbolEditor.RemoveSymbol("__TESTS__");
                SymbolEditor.RemoveSymbol("__PUBLISH__");
            }

            return result;
        }

        CICDBuildResult _Build(bool development, string jobName = "", string[] optionStrings = null, BuildTarget buildTarget = BuildTarget.NoTarget)
        {
            var result = new CICDBuildResult();

            // ビルドオプション
            if (development)
            {
                // デバッグビルド
                Log("[Build] Development Build.");
                SymbolEditor.AddSymbol("__DEBUG__");
                SymbolEditor.RemoveSymbol("__TESTS__");
                SymbolEditor.AddSymbol("__PUBLISH__");

            }
            else
            {
                // リリースビルド
                Log("[Build] Release Build.");
                SymbolEditor.RemoveSymbol("__DEBUG__");
                SymbolEditor.RemoveSymbol("__TESTS__");
                SymbolEditor.AddSymbol("__PUBLISH__");
            }

            // ビルドターゲットを変更
            if (!SwitchBuildTarget(buildTarget))
            {
                Log("[Build] SwitchBuildTarget() failed.");
                return result;
            }

            // ビルドプレイヤーオプションの設定
            BuildPlayerOptions bpo = new BuildPlayerOptions();
            {
                SettingBuildPlayerOptions(buildTarget, development, optionStrings, out bpo);

                result.BuildDirectory = GetWorkingBuildDirectory(development, buildTarget);

                // ロケーションパスの設定はココで…
                bpo.locationPathName = CreateLocationPathName(config,
                    result.BuildDirectory, buildTarget, development, jobName);

                // BuildPipeline.BuildPlayer()はフォルダが無いとエラーが出る
                BuildUtility.CreateDirectory(bpo.locationPathName);
            }

            // ビルドを実行
            BuildReport report = null;
            try
            {
                report = BuildPipeline.BuildPlayer(bpo);
                Log("[Build] Finished BuildPlayer().");

                PostprocessBuild(bpo, development, optionStrings);
                Log("[Build] Finished PostprocessBuild().");
            }
            catch (Exception e)
            {
                Log("[Build] Exception:" + e.Message);
            }
            // finally
            // {
            // }

            Log(report.ToString());

            if (report.summary.result != BuildResult.Succeeded)
            {
                Log("[Build] Build error.");
                result.BuildSucceeded = false;
            }
            else
            {
                Log("[Build] Build completed!");
                result.BuildSucceeded = true;
            }

            return result;
        }

        // ターゲット毎のビルドプレイヤーオプション設定
        public static void SettingBuildPlayerOptions(BuildTarget buildTarget, bool development, string[] optionStrings, out BuildPlayerOptions bpo)
        {
            bpo = new BuildPlayerOptions();
            bpo.target = buildTarget;
            bpo.scenes = BuildUtility.GetBuildSettingsScene().ToArray();
            bpo.options = BuildOptions.None;

            switch (buildTarget)
            {
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                case BuildTarget.StandaloneOSX:
                    if (development)
                    {
                        if (optionStrings.Contains("DevelopmentBuild"))
                        {
                            // スタンドアローンビルドはUnityEditorのコンソールにアタッチできる
                            bpo.options = BuildOptions.Development;
                        }
                    }
                    break;
            }
        }

        /*  ロケーションパス名を取得
            実行ファイルを直接ビルドするモードの場合、拡張子付きのファイルパス等を含める必要があるため。

            MEMO: platform名の文字列を直接書いているのでどうにかしたい
         */
        public static string CreateLocationPathName(CICDConfig config, string workPath, BuildTarget buildTarget, bool development, string jobName)
        {
            string app_name = "";
            string extension = "";
            string platform = "";

            switch (buildTarget)
            {
                case BuildTarget.Android:
                    extension = ".apk";
                    platform = "Android";
                    break;
                case BuildTarget.iOS:
                    platform = "iOS";
                    break;
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
                default:
                    Log("[Build] Unknown BuildTarget is " + buildTarget.ToString());
                    Debug.Assert(false);
                    break;
            }

            string ret = "";

            if (string.IsNullOrEmpty(jobName))
            {
                app_name = config.jobs.Find(obj => obj.platform == platform).application_filename;
            }
            else
            {
                Log("jobName = " + jobName);
                app_name = config.jobs.Find(obj => obj.platform == platform && obj.job_name == jobName).application_filename;
            }

            if (app_name.Length <= 0)
            {
                // アプリファイル名の設定なし
                string mode = development ? "Debug" : "Release";

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

            return workPath + ret;
        }

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
        public static bool SwitchBuildTarget(BuildTarget buildTarget)
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
                default:
                    Log("[Build] Unknown BuildTarget is " + buildTarget.ToString());
                    Debug.Assert(false);
                    break;
            }

            return EditorUserBuildSettings.SwitchActiveBuildTarget(group, buildTarget);
        }

        void PostprocessBuild(BuildPlayerOptions bpo, bool development, string[] options)
        {
            string workPath = BuildUtility.GetRootPath() + GetWorkingBuildDirectory(development, bpo.target);

            switch (bpo.target)
            {
                case BuildTarget.iOS:
#if UNITY_IOS
                    {
                        string plistPath = Path.Combine(workPath, "Info.plist");
                        // Log(plistPath);
                        PlistDocument plist = new PlistDocument();
                        plist.ReadFromFile(plistPath);

                        // 輸出コンプライアンスの設定
                        // trueでUploadしようとするとエラーになる
                        if (options.Contains("ITSAppUsesNonExemptEncryption-false"))
                        {
                            // Log("ITSAppUsesNonExemptEncryption-false");
                            plist.root.SetString("ITSAppUsesNonExemptEncryption", "false");
                        }

                        plist.WriteToFile(plistPath);
                    }
#endif
                    break;
            }
        }
    }
}
