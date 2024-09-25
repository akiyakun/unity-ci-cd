#if UNITY_STANDALONE_OSX
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace unicicd.Editor.Build
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

            return true;
        }

        public string GetBuildDirectoryName() => PlatformName;

        public string CreateLocationPathName() => "";

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