using System;
using System.Collections.Generic;
using UnityEngine;

namespace unicicd.Editor.Build
{
    [System.Serializable]
    public class CICDConfig
    {
        public const string CurrentVersion = "1.0.0";
        public const string ConfigFile = "ci-cd_config.json";

        public string version;

        [System.Serializable]
        public class Environment
        {
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

        public static CICDConfig Load()
        {
            string str = System.IO.File.ReadAllText(
                System.IO.Directory.GetCurrentDirectory() + "/" + CICDConfig.ConfigFile);
            CICDConfig ret = JsonUtility.FromJson<CICDConfig>(str);
            // Debug.Log(JsonUtility.ToJson(ret));

            // if (new Version(CurrentVersion) != new Version(ret.version))
            if (CurrentVersion != ret.version)
            {
                Debug.Log(String.Format("{0} : Current v{1}, File v{2}", ConfigFile, CurrentVersion, ret.version));
                Debug.Assert(false);
            }

            return ret;
        }
    }
}