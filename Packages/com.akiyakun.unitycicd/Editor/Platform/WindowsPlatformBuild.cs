#if UNITY_STANDALONE_WIN
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace unicicd.Editor
{
    public static class WindowsPlatformBuildOptions
    {
        public const string MonoBuild = "MonoBuild";
        public const string IL2CPPBuild = "IL2CPPBuild";
    }

    public class WindowsPlatformBuild : IPlatformBuild
    {
        public string PlatformName => "Win";
        public string ExtensionName => ".exe";
        public CICDBuildOptions BuildOptions { get; protected set; }

        string buildDirectoryName;

        // Dictionary<string, int> revertOptions = new();
        int oldArchitecture;
        ScriptingImplementation oldScriptingBackend;

        public bool Initialize(CICDBuildOptions buildOptions)
        {
            if (buildOptions == null) return false;
            BuildOptions = buildOptions;


            // ビルドディレクトリ名の生成
            // ビルドターゲットの設定も同時に設定
            {
                if (BuildOptions.HasOption(WindowsPlatformBuildOptions.MonoBuild))
                {
                    buildDirectoryName = "Win_Mono";
                    buildOptions.BuildTarget = BuildTarget.StandaloneWindows;
                    Debug.Log("MonoBuild");
                }
                else
                {
                    buildDirectoryName = "Win_IL2CPP";
                    buildOptions.BuildTarget = BuildTarget.StandaloneWindows64;
                    buildOptions.OptionStrings.Add(WindowsPlatformBuildOptions.IL2CPPBuild);
                    Debug.Log("IL2CPPBuild");
                }
            }

            return true;
        }

        public string GetBuildDirectoryName() => buildDirectoryName;

        public string CreateLocationPathName() => CICDBuilder.CreateLocationPathName(this);

        // MEMO:
        //
        public bool OnBeforeBuildProcess(CICDBuilder builder, BuildPlayerOptions bpo)
        {
            if (BuildOptions.HasOption(WindowsPlatformBuildOptions.MonoBuild))
            {
                oldScriptingBackend = PlayerSettings.GetScriptingBackend(bpo.targetGroup);
                PlayerSettings.SetScriptingBackend(bpo.targetGroup, ScriptingImplementation.Mono2x);

                // MEMO: WindowsでSetArchitecture()は使えないBuildTargetが違うのでこちらを変更する必要がある
                // oldArchitecture = PlayerSettings.GetArchitecture(bpo.targetGroup);
                // Debug.Log("oldArchitecture:" + oldArchitecture);
                // PlayerSettings.SetArchitecture(BuildTargetGroup.Standalone, 0);// 0 - x86
            }
            else if (BuildOptions.HasOption(WindowsPlatformBuildOptions.IL2CPPBuild))
            {
                Debug.Log("IL2CPPBuild 2");
                oldScriptingBackend = PlayerSettings.GetScriptingBackend(bpo.targetGroup);
                PlayerSettings.SetScriptingBackend(bpo.targetGroup, ScriptingImplementation.IL2CPP);

                // MEMO: WindowsでSetArchitecture()は使えないBuildTargetが違うのでこちらを変更する必要がある
                // oldArchitecture = PlayerSettings.GetArchitecture(bpo.targetGroup);
                // PlayerSettings.SetArchitecture(bpo.targetGroup, 1);// 1 - x64
            }

            // return false;
            return true;
        }

        public void OnAfterBuildProcess(CICDBuilder builder, BuildPlayerOptions bpo)
        {
            // Mono or IL2CPP
            // ビルドターゲットは戻さなくていいかな…
            // PlayerSettings.SetArchitecture(bpo.targetGroup, oldArchitecture);
            PlayerSettings.SetScriptingBackend(bpo.targetGroup, oldScriptingBackend);
        }
    }
}
#endif