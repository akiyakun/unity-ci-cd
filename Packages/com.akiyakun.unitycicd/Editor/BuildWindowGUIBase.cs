using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace unicicd.Editor
{
    public abstract class BuildWindowGUIBase : EditorWindow
    {
        public abstract string PlatformName { get; }

        protected EUserSettings userSettings;
        protected CICDBuildOptions buildOptions;

        bool initialized = false;
        bool showBuildMode = true;
        int buildMode;
        bool showResultDialog;

        bool isDevelopmentBuild;
        bool isWaitForManagedDebugger;
        bool isInAppDebug;
        bool isCleanBuild;
        bool isIncrementBuildNumber;

        public BuildWindowGUIBase()
        {
            userSettings = new EUserSettings();
            userSettings.Prefix += PlatformName;

            buildOptions = new();
        }

        public virtual void Initialize(CICDBuildMode mode = CICDBuildMode.Current, bool showResultDialog = true)
        {
            this.showResultDialog = showResultDialog;

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
            InnerLoadSettings();
        }

        void OnDestroy()
        {
            InnerSaveSettings();
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

            int buildNumber = GetBuildNumber();
            string currentBuildVersion = string.Format("{0}({1})", Application.version, buildNumber);
            string nextBuildVersion = string.Format("{0}({1})", Application.version, buildNumber + 1);

// #if (UNITY_EDITOR && UNITY_STANDALONE_WIN)
//             EditorGUILayout.LabelField("注意: WindowsはVersion番号の更新に対応していません");
// #else
            EditorGUILayout.LabelField("Hint: Version番号の更新はapp_config.json");
// #endif
            EditorGUILayout.LabelField("");

            isDevelopmentBuild = EditorGUILayout.Toggle("Development Build", isDevelopmentBuild);
            if (isDevelopmentBuild == false)
            {
                isWaitForManagedDebugger = false;
            }
            EditorGUI.BeginDisabledGroup(isDevelopmentBuild == false);
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

            isIncrementBuildNumber = EditorGUILayout.Toggle("ビルド番号を加算", isIncrementBuildNumber);

            if (isIncrementBuildNumber)
            {
                EditorGUILayout.LabelField(string.Format("[{2}] v{0} > v{1}", currentBuildVersion, nextBuildVersion, titleContent.text));
            }
            else
            {
                EditorGUILayout.LabelField(string.Format("[{2}] v{0}", currentBuildVersion, nextBuildVersion, titleContent.text));
            }

            OnPlatformGUI();

            // EditorGUI.BeginDisabledGroup(true);
            if (GUILayout.Button("ビルド開始"))
            {
                // ビルド番号の自動更新
                if (isIncrementBuildNumber)
                {
                    IncrementBuildNumber();
                    // Debug.Log(string.Format("{1} Start v{0}", nextBuildVersion, titleContent.text));
                }
                else
                {
                    // Debug.Log(string.Format("{1} Start v{0}", currentBuildVersion, titleContent.text));
                }

                CICDBuildResult result;

                try
                {
                    result = Build(install);
                }
                finally
                {
                }

                Close();

                if (showResultDialog)
                {
                    if (result.BuildSucceeded)
                    {
                        var dialogResult = EditorUtility.DisplayDialog(
                            "Cuccess", "ビルドが完了しました。",
                            "OK", "フォルダを開く"
                        );
                        if (dialogResult == false)
                        {
                            System.Diagnostics.Process.Start($"{BuildUtility.GetRootPath()}{result.BuildDirectory}");
                        }
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Error!", "ビルドエラー\nビルドが失敗しました。", "OK");
                    }
                }

                GUIUtility.ExitGUI();
            }

        }

        // プラットフォーム固有のGUI
        // OnGUI()内から毎回呼ばれます。
        protected virtual void OnPlatformGUI()
        {
        }

        /// ビルド開始直前に呼ばれる
        /// <returns>falseを返すとビルドが失敗します。</returns>
        protected virtual bool OnBuildStart()
        {
            return true;
        }

        CICDBuildResult Build(bool install)
        {
            var builder = new CICDBuilder();
            var ret = new CICDBuildResult();

            buildOptions.UnityDevelopmentBuild = isDevelopmentBuild;
            buildOptions.WaitForManagedDebugger = isWaitForManagedDebugger;
            buildOptions.InAppDebug = isInAppDebug;

            switch (buildMode)
            {
                // Debug
                case (int)CICDBuildMode.Debug:
                    {
                        Debug.Log("Debug Build [" + EditorUserBuildSettings.activeBuildTarget.ToString() + "]");

                        buildOptions.SetupDefaultSettings();
                        buildOptions.BuildMode = CICDBuildMode.Debug;

                        if (OnBuildStart() == false) return CICDBuildResult.CreateFailed();
                        builder.Initialize(buildOptions);
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

                        buildOptions.SetupDefaultSettings();
                        buildOptions.BuildMode = CICDBuildMode.Release;

                        if (OnBuildStart() == false) return CICDBuildResult.CreateFailed();
                        builder.Initialize(buildOptions);
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

                        buildOptions.SetupDefaultSettings();
                        buildOptions.BuildMode = CICDBuildMode.Publish;

                        if (OnBuildStart() == false) return CICDBuildResult.CreateFailed();
                        builder.Initialize(buildOptions);
                        ret = builder.Build();
                    }
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }

            return ret;
        }

        // ビルド番号を取得
        public int GetBuildNumber()
        {
#if UNITY_ANDROID
            return PlayerSettings.Android.bundleVersionCode;
#elif UNITY_IOS
            return int.Parse(PlayerSettings.iOS.buildNumber);
#elif UNITY_STANDALONE_OSX
            return int.Parse(PlayerSettings.macOS.buildNumber);
#else
            // MEMO: WindowsとかbuildNumberが無いのでローカルに保存した値を返す
            return userSettings.GetConfigInt("incrementBuildNumber", 0);
#endif
        }

        // ストア等で使用されるビルド番号を +1します
        public int IncrementBuildNumber()
        {
            int ret = 0;

#if UNITY_ANDROID
            ret = GetBuildNumber() + 1;
            PlayerSettings.Android.bundleVersionCode = ret;
#elif UNITY_IOS
            ret = GetBuildNumber() + 1;
            PlayerSettings.iOS.buildNumber = ret.ToString();
#elif UNITY_STANDALONE_OSX
            ret = GetBuildNumber() + 1;
            PlayerSettings.macOS.buildNumber = ret.ToString();
#else
            // MEMO: WindowsとかbuildNumberが無いのでローカルに保存した値を使う
            ret = GetBuildNumber() + 1;
            userSettings.SetConfigInt("incrementBuildNumber", ret);
#endif

            return ret;
        }

        protected void InnerLoadSettings()
        {
            buildMode = userSettings.GetConfigInt("buildMode", (int)CICDBuildMode.Current);

            isDevelopmentBuild = userSettings.GetConfigBool("isDevelopmentBuild", true);
            isWaitForManagedDebugger = userSettings.GetConfigBool("isWaitForManagedDebugger", false);
            isInAppDebug = userSettings.GetConfigBool("isInAppDebug", true);
            isCleanBuild = userSettings.GetConfigBool("isCleanBuild", false);

            isIncrementBuildNumber = userSettings.GetConfigBool("isIncrementBuildNumber", false);

            OnLoadSettings();
        }

        // 設定の読み込み
        protected virtual void OnLoadSettings()
        {
        }

        protected void InnerSaveSettings()
        {
            userSettings.SetConfigInt("buildMode", buildMode);

            userSettings.SetConfigBool("isDevelopmentBuild", isDevelopmentBuild);
            userSettings.SetConfigBool("isWaitForManagedDebugger", isWaitForManagedDebugger);
            userSettings.SetConfigBool("isInAppDebug", isInAppDebug);
            userSettings.SetConfigBool("isCleanBuild", isCleanBuild);

            userSettings.SetConfigBool("isIncrementBuildNumber", isIncrementBuildNumber);

            OnSaveSettings();
        }

        // 設定の保存
        protected virtual void OnSaveSettings()
        {
        }

    }
}
