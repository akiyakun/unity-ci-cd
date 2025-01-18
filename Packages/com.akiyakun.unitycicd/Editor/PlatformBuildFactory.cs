using UnityEditor;
using UnityEngine;

namespace unicicd.Editor
{
    /*
        IPlatformBuildFactoryのFactory
        拡張する場合は継承してScriptableObjectを作成してください。
     */
    // public class PlatformBuildFactory : IPlatformBuildFactory
    // public class PlatformBuildFactory : MonoBehaviour
    // [CreateAssetMenu(menuName = "App/CICD/PlatformBuildFactory", fileName = "PlatformBuildFactory")]
    public class PlatformBuildFactory : ScriptableObject
    {
        // public IPlatformBuild Create(string platformName)
        // public IPlatformBuild Create(UnityEditor.BuildTarget buildTarget)
        public virtual IPlatformBuild Create()
        {
            IPlatformBuild platformBuild = null;

#if UNITY_STANDALONE_WIN
            Debug.Log("PlatformBuildFactory: WindowsPlatformBuild");
            platformBuild = new WindowsPlatformBuild();
#elif UNITY_STANDALONE_OSX
            Debug.Log("PlatformBuildFactory: macOSPlatformBuild");
            platformBuild = new macOSPlatformBuild();
#elif UNITY_WEBGL
            Debug.Log("PlatformBuildFactory: WebGLPlatformBuild");
            platformBuild = new WebGLPlatformBuild();
#elif UNITY_ANDROID
            Debug.Log("PlatformBuildFactory: AndroidPlatformBuild");
            platformBuild = new AndroidPlatformBuild();
#elif UNITY_IOS
            Debug.Log("PlatformBuildFactory: iOSPlatformBuild");
            platformBuild = new iOSPlatformBuild();
#elif UNITY_SWITCH
            Debug.Log("PlatformBuildFactory: SwitchPlatformBuild");
            platformBuild = new SwitchPlatformBuild();
#elif UNITY_PS4
            Debug.Log("PlatformBuildFactory: PS4PlatformBuild");
            platformBuild = new PS4PlatformBuild();
            // #else
            // UnityEditor上だと UNITY_INCLUDE_TESTS がランタイムでも有効になっている
            // #if UNITY_INCLUDE_TESTS
            platformBuild = new MockPlatformBuild();
#endif

            // return null;

            if (platformBuild == null)
            {
                // Debug.Assert(false, $"Unknown platform name={platformName}");
                Debug.Assert(false, $"Unknown platform");
                return null;
            }

            return platformBuild;
        }

#if UNITY_EDITOR
        public static BuildWindowGUIBase GetWindow()
        {
            var config = CICDConfig.Load();
            return (BuildWindowGUIBase)EditorWindow.GetWindow(config.BuildSettings.PlatformBuildFactory.GetEditorGUIType(), false);
        }

        public virtual System.Type GetEditorGUIType()
        {
#if UNITY_STANDALONE_WIN
            return typeof(WindowsBuildWindowGUI);
#elif UNITY_STANDALONE_OSX
            return typeof(macOSBuildWindowGUI);
#elif UNITY_WEBGL
            return typeof(WebGLBuildWindowGUI);
#elif UNITY_ANDROID
            return typeof(AndroidBuildWindowGUI);
#elif UNITY_IOS
            return typeof(iOSBuildWindowGUI);
#elif UNITY_SWITCH
            return typeof(SwitchBuildWindowGUI);
#elif UNITY_PS4
            return typeof(PS4BuildWindowGUI);
#else
            Debug.Assert(false, "Unknown platform");
#endif
        }
#endif
    }
}
