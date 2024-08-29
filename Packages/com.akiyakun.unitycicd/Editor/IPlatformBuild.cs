using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace unicicd.Editor.Build
{
    /*
        プラットフォームの毎のビルドインターフェース
    */
    public interface IPlatformBuild
    {
        // プラットフォーム名
        string PlatformName { get; }

        CICDBuildOptions BuildOptions { get; }

        bool Initialize(CICDBuildOptions options);

        // ビルドディレクトリ名の取得
        string GetBuildDirectoryName();

        // void OnBeforeBuildProcess();
        // void OnAfterBuildProcess();
    }
}
