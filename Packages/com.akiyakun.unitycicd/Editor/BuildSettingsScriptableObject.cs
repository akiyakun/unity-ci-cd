using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace UnityCICD.Editor
{
    /*
        // アクセス方法は以下のように
        CICDConfig.Load().BuildSettings.AdditionalInfoSettings;
     */
    [CreateAssetMenu(menuName = "App/CICD/CICDBuildSettings", fileName = "CICBuildSettings")]
    [System.Serializable]
    public class BuildSettingsScriptableObject : ScriptableObject
    {
        [System.Serializable]
        public class AdditionalInfo
        {
            // gitのタイムスタンプ等の情報を含むビルド情報テキストファイルを出力するか
            public bool EnableOutputBuildInfoText;

            // 出力するビルド情報テキストファイルのパス
            public string BuildInfoTextPath = "Assets/StreamingAssets/build_info.txt";
        }

        public enum SceneType
        {
            Single,
            Additive,

            // メニューでセパレーターを表示するためのもの
            Separator,
        }

        [System.Serializable]
        public struct SceneInfo
        {
            public int Priority;
            public SceneType Type;
            public SceneAsset SceneAsset;
            public string Name;

            [NonSerialized, HideInInspector]
            public string SceneAssetPath;
        }


        [SerializeField]
        public PlatformBuildFactory PlatformBuildFactory;

        // 追加情報設定
        [SerializeField]
        public AdditionalInfo AdditionalInfoSettings;

        // 必須のシーンリスト
        // どのビルドでも必ず含まれるシーン
        [SerializeField]
        public List<SceneInfo> RequiredSceneList;

        // InAppDebug用のシーンリスト
        // InAppDebugが有効なときに含まれるシーン
        [SerializeField]
        public List<SceneInfo> InAppDebugSceneList;

        // 必須シーンのパスリストを取得
        // public string[] GetRequiredScenePathArray(bool filter = false)
        // {
        //     RequiredSceneList.Sort((a, b) => a.Priority - b.Priority);

        //     var ret = new string[RequiredSceneList.Count];
        //     for (int i = 0; i < RequiredSceneList.Count; ++i)
        //     {
        //         if (filter && RequiredSceneList[i].Priority < 0) continue;

        //         var path = UnityEditor.AssetDatabase.GetAssetPath(RequiredSceneList[i].SceneAsset);
        //         // Debug.Log(path);
        //         ret[i] = path;
        //     }

        //     return ret;
        // }

        // 必須シーンのパスリストを生成(Filter&Sort)
        public List<SceneInfo> CreateSortedRequiredSceneInfoList(bool filter = true)
        {
            return CreateSortedSceneInfoList(RequiredSceneList, filter: filter);
        }
        // public List<string> GetRequiredScenePathArraySorted()
        // {
        //     var sorted = new List<SceneInfo>(RequiredSceneList);
        //     sorted.Sort((a, b) => a.Priority - b.Priority);

        //     var ret = new List<string>(sorted.Count);
        //     sorted.ForEach(sceneInfo =>
        //     {
        //         if (sceneInfo.Priority < 0) return;
        //         ret.Add(UnityEditor.AssetDatabase.GetAssetPath(sceneInfo.SceneAsset));
        //     });

        //     return ret;
        // }

        // InAppDebugシーンのパスリストを取得
        // public string[] GetInAppDebugScenePathArray(bool filter = false)
        // {
        //     var ret = new string[InAppDebugSceneList.Count];
        //     for (int i = 0; i < InAppDebugSceneList.Count; ++i)
        //     {
        //         if (filter && RequiredSceneList[i].Priority < 0) continue;

        //         var path = UnityEditor.AssetDatabase.GetAssetPath(InAppDebugSceneList[i].SceneAsset);
        //         // Debug.Log(path);
        //         ret[i] = path;
        //     }
        //     return ret;
        // }

        // InAppDebugシーンのパスリストを生成(Filter&Sort)
        public List<SceneInfo> CreateSortedInAppDebugSceneInfoList(bool filter = true)
        {
            return CreateSortedSceneInfoList(InAppDebugSceneList, filter: filter);
        }

        // filter: trueのとき Priority < 0のものは除外する
        List<SceneInfo> CreateSortedSceneInfoList(List<SceneInfo> source, bool filter)
        {
            // プライオリティでソート
            var sorted = new List<SceneInfo>(source);
            sorted.Sort((a, b) => a.Priority - b.Priority);

            // フィルタリングして新しいリストを作る
            var ret = new List<SceneInfo>(sorted.Count);
            sorted.ForEach(sceneInfo =>
            {
                // フィルタ除外
                if (filter && sceneInfo.Priority < 0) return;

                // アセットパスを取得
                sceneInfo.SceneAssetPath = UnityEditor.AssetDatabase.GetAssetPath(sceneInfo.SceneAsset);

                ret.Add(sceneInfo);
            });

            return ret;
        }


#if UNITY_EDITOR
        void Reset()
        {
            // 新規作成時にデフォルトのPlatformBuildFactoryを設定
            PlatformBuildFactory = AssetDatabase.LoadAssetAtPath<PlatformBuildFactory>(
                $"{CICDSetup.PackageRootPath}Editor/CICDPlatformBuildFactory.asset");
        }
#endif
    }
}

