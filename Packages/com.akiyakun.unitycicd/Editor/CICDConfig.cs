using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace unicicd.Editor
{
    /*
        ci-cd_config.json
    */
    [System.Serializable]
    public class CICDConfig
    {
        public const string CurrentVersion = "1.1.0";
        public const string ConfigFile = "ci-cd_config.json";

        public string version;

        [System.Serializable]
        public class Environment
        {
            public string build_settings_path;

            public string unity_path;
            public string xcode_path;
            public string external_output_artifacts;
            public string notify_channel;
            public bool use_test;
        }
        public Environment environment;

        [System.Serializable]
        public class Jobs
        {
            public string platform;
            public bool enable;
            public string agent;
            public bool cleanup_build;
            public string application_filename;
            public string job_name;
        }
        public List<Jobs> jobs;

        public BuildSettings BuildSettings;

        public static CICDConfig Load()
        {
            string str = System.IO.File.ReadAllText(
                System.IO.Directory.GetCurrentDirectory() + "/" + CICDConfig.ConfigFile);
            CICDConfig ret = JsonUtility.FromJson<CICDConfig>(str);
            // Debug.Log(JsonUtility.ToJson(ret));

            // if (new Version(CurrentVersion) != new Version(ret.version))
            if (CurrentVersion != ret.version)
            {
                Debug.LogError(String.Format("{0} : Current v{1}, File v{2}", ConfigFile, CurrentVersion, ret.version));
            }

            // BuildSettings読み込み
            {
                var buildSettings = AssetDatabase.LoadAssetAtPath<BuildSettings>(ret.environment.build_settings_path);
                Debug.Assert(buildSettings != null, "BuildSettings.asset ScriptableObjectを作成してください。");

                // 必須シーンチェック
                Debug.Assert(buildSettings.RequiredSceneList != null);
                if( buildSettings.RequiredSceneList != null)
                {
                    Debug.Assert(buildSettings.RequiredSceneList.Count > 0);
                    foreach (var scene in buildSettings.RequiredSceneList)
                    {
                        Debug.Assert(scene != null, "Required scene is null reference");
                    }
                }

                // InAppDebugシーンチェック
                Debug.Assert(buildSettings.InAppDebugSceneList != null);
                if( buildSettings.InAppDebugSceneList != null)
                {
                    Debug.Assert(buildSettings.InAppDebugSceneList.Count > 0);
                    foreach (var scene in buildSettings.InAppDebugSceneList)
                    {
                        Debug.Assert(scene != null, "InAppDebug scene is null reference");
                    }
                }

                ret.BuildSettings = buildSettings;
            }

            return ret;
        }
    }
}