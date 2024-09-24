using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using unicicd.Editor;

namespace unicicd.Editor.Build
{
    public abstract class BuildWindowGUIBase : EditorWindow
    {
        public abstract string PlatformName { get; }

        bool initialized = false;
        bool showBuildMode = true;
        // CICDBuildOptions.BuildMode buildMode;
        int buildMode;

        bool incrementBuildNumber = true;

        bool isDevelopmentBuild = true;
        bool isWaitForManagedDebugger = false;
        bool isInAppDebug = true;
        bool isCleanBuild = true;

        // BuildMenu.BuildMode BuildMode { get; set; }
        // string jobName;
        // public CICDBuildOptions Options { get; private set; }

        public System.Action OnBuildBegin;
        public System.Action<CICDBuildResult> OnBuildEnd;

        public void Initialize(CICDBuildMode mode = CICDBuildMode.Current)
        {
            switch (mode)
            {
                case CICDBuildMode.Current:
                    if (buildMode <= 0) buildMode = 1;
                    break;
                case CICDBuildMode.Debug:
                    showBuildMode = false;
                    buildMode = (int)mode;
                    isDevelopmentBuild = true;
                    isInAppDebug = true;
                    break;
                case CICDBuildMode.Release:
                    showBuildMode = false;
                    buildMode = (int)mode;
                    isDevelopmentBuild = false;
                    isInAppDebug = true;
                    break;
                case CICDBuildMode.Publish:
                    showBuildMode = false;
                    buildMode = (int)mode;
                    isDevelopmentBuild = false;
                    isWaitForManagedDebugger = false;
                    isInAppDebug = false;
                    isCleanBuild = true;
                    break;
            }

            initialized = true;
        }

        void OnEnable()
        {
            OnLoadSettings();
        }

        void OnDestroy()
        {
            OnSaveSettings();
        }

#pragma warning disable 0414
        bool foldout = false;
#pragma warning restore 0414

        private UnityEngine.GUIContent[] buildModeDisplayedOptions = new[]
        {
            new UnityEngine.GUIContent("Debug"),
            new UnityEngine.GUIContent("Release"),
            new UnityEngine.GUIContent("Publish"),
        };


        bool install = false;
        private void OnGUI()
        {
            if (initialized == false) return;

            // 現在のサイズ表示
            // EditorGUILayout.LabelField($"現在のサイズ : {position.size}");
            // minSize = EditorGUILayout.Vector2Field("最小サイズ", minSize);
            // maxSize = EditorGUILayout.Vector2Field("最大サイズ", maxSize);
            minSize = new Vector2(400, 300);
            maxSize = new Vector2(400, 300);

            if (showBuildMode)
            {
                buildMode = UnityEditor.EditorGUILayout.Popup(
                    label: new UnityEngine.GUIContent("Build Mode"),
                    selectedIndex: buildMode - 1,
                    displayedOptions: buildModeDisplayedOptions
                ) + 1;
            }

            int buildNumber = BuildUtility.GetBuildNumber();
            string currentBuildVersion = string.Format("{0}({1})", Application.version, buildNumber);
            string nextBuildVersion = string.Format("{0}({1})", Application.version, buildNumber + 1);

#if (UNITY_EDITOR && UNITY_STANDALONE_WIN)
            EditorGUILayout.LabelField("注意: WindowsはVersion番号の更新に対応していません");
#else
            EditorGUILayout.LabelField("Hint: Version番号の更新はapp_config.json");
#endif
            EditorGUILayout.LabelField("");

            EditorGUI.BeginDisabledGroup(true);
            isDevelopmentBuild = EditorGUILayout.Toggle("Development Build", isDevelopmentBuild);
            isWaitForManagedDebugger = EditorGUILayout.Toggle("Wait For Managed Debugger", isWaitForManagedDebugger);
            EditorGUI.EndDisabledGroup();

            isInAppDebug = EditorGUILayout.Toggle("In App Debug", isInAppDebug);
            isCleanBuild = EditorGUILayout.Toggle("Clean Build", isCleanBuild);

// #if (UNITY_EDITOR && UNITY_ANDROID)
//             if (BuildMode == BuildMenu.BuildMode.AndroidMono) install = true;

//             foldout = EditorGUILayout.Foldout(foldout, "Android");
//             if (foldout)
//             {
//                 install = EditorGUILayout.Toggle("APKインストール", install);
//             }
//             EditorGUILayout.Space();
// #endif

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

        }

        CICDBuildResult Build(bool install)
        {
            var options = new CICDBuildOptions();
            var builder = new CICDBuilder();
            List<string> optionStrings = new List<string>();
            var ret = new CICDBuildResult();

            options.UnityDevelopmentBuild = isDevelopmentBuild;
            options.WaitForManagedDebugger = isWaitForManagedDebugger;
            options.InAppDebug = isInAppDebug;

            switch (buildMode)
            {
                // Debug
                case (int)CICDBuildMode.Debug:
                    {
                        Debug.Log("Debug Build [" + EditorUserBuildSettings.activeBuildTarget.ToString() + "]");

                        optionStrings.Add("ITSAppUsesNonExemptEncryption-false");

                        options.SetupDefaultSettings();
                        options.BuildMode = CICDBuildMode.Debug;
                        options.OptionStrings.AddRange(optionStrings);
                        builder.Initialize(options);
                        ret = builder.Build();

// #if (UNITY_EDITOR && UNITY_ANDROID)
//                         if (install) BuildMenu.AndroidInstallAPK();
// #endif
                    }
                    break;

                // Release
                case (int)CICDBuildMode.Release:
                    {
                        Debug.Log("Release Build [" + EditorUserBuildSettings.activeBuildTarget.ToString() + "]");

                        options.SetupDefaultSettings();
                        options.BuildMode = CICDBuildMode.Release;
                        options.OptionStrings.AddRange(optionStrings);
                        builder.Initialize(options);
                        ret = builder.Build();

// #if (UNITY_EDITOR && UNITY_ANDROID)
//                         if (install) BuildMenu.AndroidInstallAPK();
// #endif
                    }
                    break;

                // Publish
                case (int)CICDBuildMode.Publish:
                    {
                        Debug.Log("Publish Build [" + EditorUserBuildSettings.activeBuildTarget.ToString() + "]");

                        options.SetupDefaultSettings();
                        options.BuildMode = CICDBuildMode.Publish;
                        options.OptionStrings.AddRange(optionStrings);
                        builder.Initialize(options);
                        ret = builder.Build();
                    }
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }

            return ret;
        }

        // 設定の読み込み
        protected virtual void OnLoadSettings()
        {
            buildMode = EUserSettings.GetConfigInt("buildMode", (int)CICDBuildMode.Current);

            isDevelopmentBuild = EUserSettings.GetConfigBool("isDevelopmentBuild", true);
            isWaitForManagedDebugger = EUserSettings.GetConfigBool("isWaitForManagedDebugger", false);
            isInAppDebug = EUserSettings.GetConfigBool("isInAppDebug", true);
            isCleanBuild = EUserSettings.GetConfigBool("isCleanBuild", false);
        }

        // 設定の保存
        protected virtual void OnSaveSettings()
        {
            EUserSettings.SetConfigInt("buildMode", buildMode);

            EUserSettings.SetConfigBool("isDevelopmentBuild", isDevelopmentBuild);
            EUserSettings.SetConfigBool("isWaitForManagedDebugger", isWaitForManagedDebugger);
            EUserSettings.SetConfigBool("isInAppDebug", isInAppDebug);
            EUserSettings.SetConfigBool("isCleanBuild", isCleanBuild);
        }

    }
}
