#if UNITY_WEBGL
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace unicicd.Editor.Build
{
    public class WebGLPlatformBuild : IPlatformBuild
    {
        public string PlatformName => "WebGL";
        public CICDBuildOptions BuildOptions { get; protected set; }

        public bool Initialize(CICDBuildOptions options)
        {
            if (options == null) return false;
            BuildOptions = options;

            return true;
        }

        public string GetBuildDirectoryName() => PlatformName;

    }
}
#endif