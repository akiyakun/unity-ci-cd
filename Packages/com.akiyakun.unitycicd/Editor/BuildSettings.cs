using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace unicicd.Editor
{
    [CreateAssetMenu(menuName = "App/CICD/BuildSettings", fileName = "CICDBuildSettings")]
    [System.Serializable]
    public class BuildSettings : ScriptableObject
    {
        [SerializeField]
        public PlatformBuildFactory PlatformBuildFactory;

        // 必須のシーンリスト
        // どのビルドでも必ず含まれるシーン
        [SerializeField]
        public List<SceneAsset> RequiredSceneList;

        // InAppDebug用のシーンリスト
        // InAppDebugが有効なときに含まれるシーン
        [SerializeField]
        public List<SceneAsset> InAppDebugSceneList;

        // 必須シーンのパスリストを取得
        public string[] GetRequiredScenePathArray()
        {
            var ret = new string[RequiredSceneList.Count];
            for (int i = 0; i < RequiredSceneList.Count; ++i)
            {
                var path = UnityEditor.AssetDatabase.GetAssetPath(RequiredSceneList[i]);
                // Debug.Log(path);
                ret[i] = path;
            }
            return ret;
        }

        // InAppDebugシーンのパスリストを取得
        public string[] GetInAppDebugScenePathArray()
        {
            var ret = new string[InAppDebugSceneList.Count];
            for (int i = 0; i < InAppDebugSceneList.Count; ++i)
            {
                var path = UnityEditor.AssetDatabase.GetAssetPath(InAppDebugSceneList[i]);
                // Debug.Log(path);
                ret[i] = path;
            }
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

