#if UNITY_STANDALONE_OSX
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace unicicd.Editor
{
    public class macOSPlatformBuild : IPlatformBuild
    {
        public string PlatformName => "macOS";
        public string ExtensionName => ".app";
        public CICDBuildOptions BuildOptions { get; protected set; }

        public bool Initialize(CICDBuildOptions buildOptions)
        {
            if (buildOptions == null) return false;
            BuildOptions = buildOptions;

            buildOptions.BuildTarget = BuildTarget.StandaloneOSX;

            // Debug.Log($"macOSPlatformBuild: Initialize() {CreateLocationPathName()}");

            return true;
        }

        public string GetBuildDirectoryName() => PlatformName;

        // MEMO: macOSビルドはLocationに.app拡張子を付ける必要がある
        // public string CreateLocationPathName() => ".app";
        public string CreateLocationPathName() => CICDBuilder.CreateLocationPathName(this);

        public bool OnBeforeBuildProcess(CICDBuilder builder, BuildPlayerOptions bpo)
        {
            // Debug.Log("macOSPlatformBuild: OnBeforeBuildProcess()");
            return true;
        }

        public void OnAfterBuildProcess(CICDBuilder builder, BuildPlayerOptions bpo)
        {
            // Debug.Log("macOSPlatformBuild: OnAfterBuildProcess()");
        }
    }
}
#endif