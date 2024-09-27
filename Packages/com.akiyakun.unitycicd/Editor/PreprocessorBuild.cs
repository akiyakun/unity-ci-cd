using System;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace unicicd.Editor
{
    // .NET プロファイルについて
    // https://docs.unity3d.com/ja/2021.3/Manual/dotnetProfileSupport.html
    public class PreprocessorBuild : IPreprocessBuildWithReport
    {
        public int callbackOrder { get { return 0; } }

#if (UNITY_EDITOR && UNITY_ANDROID)
        public static bool AndroidMonoBuild = false;
#endif

        public void OnPreprocessBuild(BuildReport report)
        {
            // Debug.Log("OnPreprocessBuild for target " + report.summary.platform + " at path " + report.summary.outputPath);

            // 各ビルドターゲット毎の設定
            BuildTarget buildTarget = report.summary.platform;
            Debug.Log("[Build] OnPreprocessBuild() BuildTarget: " + buildTarget.ToString());
            switch (buildTarget)
            {
                case BuildTarget.Android:
                    AndroidSettings();
                    break;
                case BuildTarget.iOS:
                    iOSSettings();
                    break;
                case BuildTarget.StandaloneWindows:
                    Win32Settings();
                    break;
                case BuildTarget.StandaloneWindows64:
                    Win64Settings();
                    break;
                case BuildTarget.StandaloneOSX:
                    break;
                case BuildTarget.WebGL:
                    break;
                case BuildTarget.Switch:
                    break;
                case BuildTarget.PS4:
                    break;
                default:
                    Console.WriteLine("[Build] Unknown BuildTarget is " + buildTarget.ToString());
                    Debug.Assert(false);
                    break;
            }
        }

        public static void ApplyApiCompatibilityLevel(BuildTargetGroup group)
        {
#if UNITY_2021_2_1_OR_NEWER
            // .NET Standard 2.1
			PlayerSettings.SetApiCompatibilityLevel(group, ApiCompatibilityLevel.NET_Standard);
#else
            PlayerSettings.SetApiCompatibilityLevel(group, ApiCompatibilityLevel.NET_Standard_2_0);
#endif
        }

        public static void AndroidSettings()
        {
            BuildTargetGroup group = BuildTargetGroup.Android;
            PlayerSettings.SetScriptingBackend(group, ScriptingImplementation.IL2CPP);
            // PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            ApplyApiCompatibilityLevel(group);

#if (UNITY_EDITOR && UNITY_ANDROID)
            if (AndroidMonoBuild)
            {
                // Monoでビルド
                PlayerSettings.SetScriptingBackend(group, ScriptingImplementation.Mono2x);
                // Monoは64bitビルド非対応
                PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7;
            }
#endif
        }

        public static void iOSSettings()
        {
            BuildTargetGroup group = BuildTargetGroup.iOS;
            PlayerSettings.SetScriptingBackend(group, ScriptingImplementation.IL2CPP);
            ApplyApiCompatibilityLevel(group);

            PlayerSettings.SetArchitecture(BuildTargetGroup.iOS, (int)AppleMobileArchitecture.ARM64);
            PlayerSettings.iOS.appleEnableAutomaticSigning = true;

        }

        public static void Win32Settings()
        {
            BuildTargetGroup group = BuildTargetGroup.Standalone;
            // PlayerSettings.SetScriptingBackend(group, ScriptingImplementation.Mono2x);
            // PlayerSettings.SetScriptingBackend(group, ScriptingImplementation.IL2CPP);
            ApplyApiCompatibilityLevel(group);
        }

        public static void Win64Settings()
        {
            BuildTargetGroup group = BuildTargetGroup.Standalone;
            // PlayerSettings.SetScriptingBackend(group, ScriptingImplementation.Mono2x);
            // PlayerSettings.SetScriptingBackend(group, ScriptingImplementation.IL2CPP);
            ApplyApiCompatibilityLevel(group);
        }

        public static void macOSSettings()
        {
            BuildTargetGroup group = BuildTargetGroup.Standalone;
            // PlayerSettings.SetScriptingBackend(group, ScriptingImplementation.Mono2x);
            PlayerSettings.SetScriptingBackend(group, ScriptingImplementation.IL2CPP);
            ApplyApiCompatibilityLevel(group);

            // PlayerSettings.SetArchitecture(BuildTargetGroup.Standalone, 1);     // 64bit
        }
    }
}
