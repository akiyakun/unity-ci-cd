using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace unicicd.Editor
{
    /*
        プラットフォームの毎のビルドインターフェース
    */
    public interface IPlatformBuild
    {
        // プラットフォーム名
        public string PlatformName { get; }

        // 拡張子
        public string ExtensionName { get; }

        public CICDBuildOptions BuildOptions { get; }

        public bool Initialize(CICDBuildOptions buildOptions);

        // ビルドディレクトリ名の取得
        public string GetBuildDirectoryName();

        public string CreateLocationPathName();

        // return false is cancel build.
        public bool OnBeforeBuildProcess(CICDBuilder builder, BuildPlayerOptions bpo);

        public void OnAfterBuildProcess(CICDBuilder builder, BuildPlayerOptions bpo);
    }
}
