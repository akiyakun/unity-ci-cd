using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace unicicd.Editor.Build
{
    public class MockPlatformBuild : IPlatformBuild
    {
        public string PlatformName => "Mock";
        public CICDBuildOptions BuildOptions { get; protected set; }

        string buildDirectoryName;

        public bool Initialize(CICDBuildOptions options)
        {
            if (options == null) return false;
            BuildOptions = options;

            // ビルドディレクトリ名の生成
            buildDirectoryName = $"Mock_Test";

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
