#if UNITY_WEBGL
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace unicicd.Editor
{
    public class WebGLPlatformBuild : IPlatformBuild
    {
        public string PlatformName => "WebGL";
        public string ExtensionName => "";
        public CICDBuildOptions BuildOptions { get; protected set; }

        public bool Initialize(CICDBuildOptions buildOptions)
        {
            if (buildOptions == null) return false;
            BuildOptions = buildOptions;

            buildOptions.BuildTarget = BuildTarget.WebBL;

            return true;
        }

        public string GetBuildDirectoryName() => PlatformName;

        public string CreateLocationPathName() => PlatformName;

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