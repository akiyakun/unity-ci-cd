#if UNITY_IOS
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEditor.iOS.Xcode;

namespace unicicd.Editor.Build
{
    public class iOSPlatformBuild : IPlatformBuild
    {
        public string PlatformName => "iOS";
        public CICDBuildOptions BuildOptions { get; protected set; }

        string buildDirectoryName;

        public bool Initialize(CICDBuildOptions options)
        {
            if (options == null) return false;
            BuildOptions = options;

            // ビルドディレクトリ名の生成
            buildDirectoryName = $"{PlatformName}/xcode";

            return true;
        }

        public string GetBuildDirectoryName() => buildDirectoryName;

        public bool OnBeforeBuildProcess(CICDBuilder builder, BuildPlayerOptions bpo)
        {
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