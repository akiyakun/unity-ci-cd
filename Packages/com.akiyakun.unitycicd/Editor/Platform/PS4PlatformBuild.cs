#if UNITY_PS4
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace unicicd.Editor
{
    public class PS4PlatformBuild : IPlatformBuild
    {
        public virtual string PlatformName => "PS4";
        public virtual string ExtensionName => "";
        public virtual CICDBuildOptions BuildOptions { get; protected set; }

        public virtual bool Initialize(CICDBuildOptions buildOptions)
        {
            if (buildOptions == null) return false;
            BuildOptions = buildOptions;

            buildOptions.BuildTarget = BuildTarget.PS4;

            return true;
        }

        public virtual string GetBuildDirectoryName() => PlatformName;

        public virtual string CreateLocationPathName() => CICDBuilder.CreateLocationPathName(this);

        public virtual bool OnBeforeBuildProcess(CICDBuilder builder, BuildPlayerOptions bpo)
        {
            return true;
        }

        public virtual void OnAfterBuildProcess(CICDBuilder builder, BuildPlayerOptions bpo)
        {
        }
    }
}
#endif