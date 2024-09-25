#if UNITY_PS4
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace unicicd.Editor.Build
{
    public class PS4PlatformBuild : IPlatformBuild
    {
        public string PlatformName => "PS4";
        public string ExtensionName => "";
        public CICDBuildOptions BuildOptions { get; protected set; }

        public bool Initialize(CICDBuildOptions buildOptions)
        {
            if (buildOptions == null) return false;
            BuildOptions = buildOptions;

            buildOptions.BuildTarget = BuildTarget.PS4;

            return true;
        }

        public string GetBuildDirectoryName() => PlatformName;

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
#endif