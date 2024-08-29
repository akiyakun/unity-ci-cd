using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace unicicd.Editor.Build
{
    [CreateAssetMenu(menuName = "App/CICD/BuildSettings")]
    [System.Serializable]
    public class BuildSettings : ScriptableObject
    {
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
    }
}

