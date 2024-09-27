#if UNITY_SWITCH
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace unicicd.Editor
{
    public class SwitchPlatformBuild : IPlatformBuild
    {
        public string PlatformName => "Switch";
        public string ExtensionName => "";
        public CICDBuildOptions BuildOptions { get; protected set; }

        public bool Initialize(CICDBuildOptions buildOptions)
        {
            if (buildOptions == null) return false;
            BuildOptions = buildOptions;

            buildOptions.BuildTarget = BuildTarget.Switch;

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