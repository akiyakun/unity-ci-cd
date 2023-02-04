using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using unicicd.Editor;

namespace unicicd.Editor.Build
{
    public class BuildWindow : EditorWindow
    {
        bool incrementBuildNumber = true;
        bool developmentBuild = false;

        BuildMenu.BuildMode BuildMode { get; set; }
        string jobName;

        public System.Action OnBuildBegin;
        public System.Action<CICDBuildResult> OnBuildEnd;

        public void Initialize(BuildMenu.BuildMode buildMode, string jobName_ = "")
        {
            BuildMode = buildMode;
            jobName = jobName_;

            switch (BuildMode)
            {
                case BuildMenu.BuildMode.Debug:
                    titleContent.text = "Debug Build";
                    break;
                case BuildMenu.BuildMode.Release:
                    titleContent.text = "Release Build";
                    break;
                case BuildMenu.BuildMode.AndroidMono:
                    titleContent.text = "Android Mono Build";
                    incrementBuildNumber = false;
                    break;
            }
        }

#pragma warning disable 0414
        bool foldout = false;
#pragma warning restore 0414

        bool install = false;
        private void OnGUI()
        {
            int buildNumber = BuildUtility.GetBuildNumber();
            string currentBuildVersion = string.Format("{0}({1})", Application.version, buildNumber);
            string nextBuildVersion = string.Format("{0}({1})", Application.version, buildNumber + 1);

#if (UNITY_EDITOR && UNITY_STANDALONE_WIN)
            EditorGUILayout.LabelField("注意: WindowsはVersion番号の更新に対応していません");
#else
            EditorGUILayout.LabelField("Hint: Version番号の更新はapp_config.json");
#endif
            EditorGUILayout.LabelField("");

            // Development Build
            if (BuildMode == BuildMenu.BuildMode.Debug)
            {
                developmentBuild = EditorGUILayout.Toggle("Development Build", developmentBuild);
            }

#if (UNITY_EDITOR && UNITY_ANDROID)
            if (BuildMode == BuildMenu.BuildMode.AndroidMono) install = true;

            foldout = EditorGUILayout.Foldout(foldout, "Android");
            if (foldout)
            {
                install = EditorGUILayout.Toggle("APKインストール", install);
            }
            EditorGUILayout.Space();
#endif

            incrementBuildNumber = EditorGUILayout.Toggle("ビルド番号を加算", incrementBuildNumber);

            if (incrementBuildNumber)
            {
                EditorGUILayout.LabelField(string.Format("[{2}] v{0} > v{1}", currentBuildVersion, nextBuildVersion, titleContent.text));
            }
            else
            {
                EditorGUILayout.LabelField(string.Format("[{2}] v{0}", currentBuildVersion, nextBuildVersion, titleContent.text));
            }

            // EditorGUI.BeginDisabledGroup(true);
            if (GUILayout.Button("ビルド開始"))
            {
                // ビルド番号の自動更新
                if (incrementBuildNumber)
                {
                    BuildUtility.IncrementBuildNumber();
                    Debug.Log(string.Format("{1} Start v{0}", nextBuildVersion, titleContent.text));
                }
                else
                {
                    Debug.Log(string.Format("{1} Start v{0}", currentBuildVersion, titleContent.text));
                }

                var ret = new CICDBuildResult();
                try
                {
                    OnBuildBegin?.Invoke();
                    ret = Build(install);
                }
                finally
                {
                    OnBuildEnd?.Invoke(ret);
                }

                Close();
            }

            // EditorGUI.EndDisabledGroup();

        }

        CICDBuildResult Build(bool install)
        {
            var builder = new CICDBuilder();
            List<string> optionStrings = new List<string>();
            var ret = new CICDBuildResult();

            if (developmentBuild) optionStrings.Add("DevelopmentBuild");

            switch (BuildMode)
            {
                case BuildMenu.BuildMode.Debug:
                    {
                        Debug.Log("Debug Build [" + EditorUserBuildSettings.activeBuildTarget.ToString() + "]");

                        optionStrings.Add("ITSAppUsesNonExemptEncryption-false");

                        ret = builder.Build(
                            development: true,
                            jobName: jobName,
                            optionStrings: optionStrings.ToArray(),
                            buildTarget: EditorUserBuildSettings.activeBuildTarget
                        );

#if (UNITY_EDITOR && UNITY_ANDROID)
                        if (install) BuildMenu.AndroidInstallAPK();
#endif
                    }
                    break;
                case BuildMenu.BuildMode.Release:
                    {
                        Debug.Log("Release Build [" + EditorUserBuildSettings.activeBuildTarget.ToString() + "]");

                        ret = builder.Build(
                            development: false,
                            jobName: jobName,
                            optionStrings: optionStrings.ToArray(),
                            buildTarget: EditorUserBuildSettings.activeBuildTarget
                        );

#if (UNITY_EDITOR && UNITY_ANDROID)
                        if (install) BuildMenu.AndroidInstallAPK();
#endif
                    }
                    break;
                case BuildMenu.BuildMode.AndroidMono:
#if (UNITY_EDITOR && UNITY_ANDROID)
                    {
                        // MonoビルドフラグをON
                        PreprocessorBuild.AndroidMonoBuild = true;

                        try
                        {
                            ret = builder.Build(
                                development: true,
                                jobName: jobName,
								optionStrings: optionStrings.ToArray(),
                                buildTarget: BuildTarget.Android
                            );

                            if (install) BuildMenu.AndroidInstallAPK();
                        }
                        finally
                        {
                            // ビルド設定を戻す
                            PreprocessorBuild.AndroidMonoBuild = false;
                            PreprocessorBuild.AndroidSettings();
                        }
                    }
#endif
                    break;
            }

            return ret;
        }
    }
}
