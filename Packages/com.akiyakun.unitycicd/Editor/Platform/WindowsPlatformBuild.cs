#if UNITY_STANDALONE_WIN
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace unicicd.Editor.Build
{
    public class WindowsPlatformBuild : IPlatformBuild
    {
        public string PlatformName => "Win";
        public CICDBuildOptions BuildOptions { get; protected set; }

        string buildDirectoryName;

        public bool Initialize(CICDBuildOptions options)
        {
            if (options == null) return false;
            BuildOptions = options;

            // ビルドディレクトリ名の生成
            {
                ScriptingImplementation backend = PlayerSettings.GetScriptingBackend(BuildTargetGroup.Standalone);
                buildDirectoryName = $"Win_{backend.ToString()}";
            }

            return true;
        }

        public string GetBuildDirectoryName() => buildDirectoryName;

        public bool OnBeforeBuildProcess(CICDBuilder builder, BuildPlayerOptions bpo)
        {
            return true;
        }

        public void OnAfterBuildProcess(CICDBuilder builder, BuildPlayerOptions bpo)
        {
        }
    }
}
#endif