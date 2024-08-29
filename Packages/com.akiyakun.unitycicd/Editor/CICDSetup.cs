using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace unicicd.Editor.Build
{
    [System.Serializable]
    public class CICDSetup
    {
        public const string PackageRootPath = "Packages/com.akiyakun.unitycicd/";
        public const string VersionFile = "tools/ci-cd/version";
        public const string CICDConfigFile = "ci-cd_config.json";

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
            // versionファイルが無い、バージョンが異なる場合にコピーする
            try
            {
                Version ver = Version.Parse(File.ReadAllText(BuildUtility.GetRootPath() + VersionFile));
                CICDSetup json = CICDSetup.Load();

                if (ver != new Version(json.version))
                {
                    // Debug.Log("copy. different a version");
                    CopyExternalFiles();
                }
            }
            catch
            {
                // Debug.Log("copy. not found a file");
                CopyExternalFiles();
            }
        }

        /*
            toolsフォルダの中身等をプロジェクトにコピーします。
        */
        static void CopyExternalFiles()
        {
            string prjRoot = BuildUtility.GetRootPath();
            string externalPath = Path.GetFullPath(PackageRootPath) + "/External~";

            // ci-cd_config.jsonファイルが無ければコピー
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
