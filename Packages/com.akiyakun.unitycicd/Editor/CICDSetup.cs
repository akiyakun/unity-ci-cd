using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityCICD.Editor
{
    [System.Serializable]
    public class CICDSetup
    {
        public const string PackageRootPath = "Packages/com.akiyakun.unitycicd/";
        public const string VersionFile = "tools/cicd/cicd_version";
        public const string CICDConfigFile = "cicd_config.json";

        // string name = "";
        public string version = "";


        public static CICDSetup Load()
        {
            string path = Path.GetFullPath(PackageRootPath + "package.json");
            string str = System.IO.File.ReadAllText(path);
            CICDSetup ret = JsonUtility.FromJson<CICDSetup>(str);
            // Debug.Log(JsonUtility.ToJson(ret));
            return ret;
        }

        [InitializeOnLoadMethod]
        static void Setup()
        {
            CheckExternalFile();
        }

        static void CheckExternalFile()
        {
            string versionFilePath = BuildUtility.GetRootPath() + VersionFile;
            // Debug.Log($"CheckExternalFile: versionFilePath={versionFilePath}");

            // versionファイルが無い場合はコピーする
            if (File.Exists(versionFilePath) == false)
            {
                CopyExternalFiles();
                return;
            }

            // バージョンが異なる場合コピーする
            try
            {
                Version ver = Version.Parse(File.ReadAllText(versionFilePath));
                if (ver != new Version(CICDConfig.CurrentVersion))
                {
                    // Debug.Log("copy. different a version");
                    CopyExternalFiles();

                    Debug.Assert(Version.Parse(File.ReadAllText(versionFilePath)) == new Version(CICDConfig.CurrentVersion),
                        $"{VersionFile} ファイルのバージョンを更新してください。");
                }
            }
            catch
            {
                Debug.Assert(false);
                // CopyExternalFiles();
            }
        }

        /*
            toolsフォルダの中身等をプロジェクトにコピーします。
        */
        static void CopyExternalFiles()
        {
            string prjRoot = BuildUtility.GetRootPath();
            string externalPath = Path.GetFullPath(PackageRootPath) + "/External~";

            // cicd_config.jsonファイルが無ければコピー
            string configFile = prjRoot + CICDConfigFile;
            // Debug.Log(configFile);
            if (!File.Exists(configFile))
            {
                // Debug.Log("copy config .json");
                File.Copy(externalPath + "/" + CICDConfigFile, configFile);
            }

            // toolsフォルダのコピー
            BuildUtility.DirectoryCopy(externalPath + "/tools", BuildUtility.GetRootPath() + "tools", true);
            // フォルダ以下全部上書き
            // BuildUtility.DirectoryCopy(src, BuildUtility.GetRootPath(), true);
        }

    }
}
