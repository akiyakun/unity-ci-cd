#if UNITY_IOS
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEditor.iOS.Xcode;

namespace unicicd.Editor
{
    public class iOSPlatformBuild : IPlatformBuild
    {
        public string PlatformName => "iOS";
        public string ExtensionName => "";
        public CICDBuildOptions BuildOptions { get; protected set; }

        string buildDirectoryName;

        public bool Initialize(CICDBuildOptions buildOptions)
        {
            if (buildOptions == null) return false;
            BuildOptions = buildOptions;

            buildOptions.BuildTarget = BuildTarget.iOS;

            // ビルドディレクトリ名の生成
            buildDirectoryName = $"{PlatformName}/xcode";

            return true;
        }

        public string GetBuildDirectoryName() => buildDirectoryName;

        public string GetLocationPathName() => buildDirectoryName;

        public bool OnBeforeBuildProcess(CICDBuilder builder, BuildPlayerOptions bpo)
        {
            // optionStrings.Add("ITSAppUsesNonExemptEncryption-false");
            return true;
        }

        public void OnAfterBuildProcess(CICDBuilder builder, BuildPlayerOptions bpo)
        {
            Debug.Assert(bpo.target == BuildTarget.iOS);

            string workPath = BuildUtility.PathCombine(BuildUtility.GetRootPath(), builder.WorkingBuildDirectory);

            {
                string plistPath = BuildUtility.PathCombine(workPath, "Info.plist");
                // Log(plistPath);
                PlistDocument plist = new PlistDocument();
                plist.ReadFromFile(plistPath);

                // 輸出コンプライアンスの設定
                // trueでUploadしようとするとエラーになる
                if (builder.BuildOptions.OptionStrings.Contains("ITSAppUsesNonExemptEncryption-false"))
                {
                    // Log("ITSAppUsesNonExemptEncryption-false");
                    plist.root.SetString("ITSAppUsesNonExemptEncryption", "false");
                }

                plist.WriteToFile(plistPath);
            }

        }
    }
}
#endif