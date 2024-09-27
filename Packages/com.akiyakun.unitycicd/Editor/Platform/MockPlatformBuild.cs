using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace unicicd.Editor
{
    public class MockPlatformBuild : IPlatformBuild
    {
        public string PlatformName => "Mock";
        public string ExtensionName => ".exe";
        public CICDBuildOptions BuildOptions { get; protected set; }

        string buildDirectoryName;

        public bool Initialize(CICDBuildOptions buildOptions)
        {
            if (buildOptions == null) return false;
            BuildOptions = buildOptions;

            // 現在の設定を反映
            buildOptions.ApplyCurrentBuildTarget();

            // ビルドディレクトリ名の生成
            buildDirectoryName = $"Mock_Test";

            return true;
        }

        public string GetBuildDirectoryName() => buildDirectoryName;

        public string CreateLocationPathName() => CICDBuilder.CreateLocationPathName(this);

        public bool OnBeforeBuildProcess(CICDBuilder builder, BuildPlayerOptions bpo)
        {
            return true;
        }

        public void OnAfterBuildProcess(CICDBuilder builder, BuildPlayerOptions bpo)
        {
        }
    }
}
