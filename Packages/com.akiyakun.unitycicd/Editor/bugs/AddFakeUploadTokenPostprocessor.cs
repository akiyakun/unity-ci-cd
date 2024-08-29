#if UNITY_IOS
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.iOS.Xcode;
using UnityEngine;

// こんな感じのエラーが出るときの一時的対処用
//
// The following build commands failed:
// PhaseScriptExecution Process\ symbols /BUILD_PATH/Library/Developer/Xcode/DerivedData/
// Unity-iPhone-gjomesfkojewdbcqkxboenqtbzba/Build/Intermediates.noindex/ArchiveIntermediates/Unity-iPhone/
// IntermediateBuildFilesPath/Unity-iPhone.build/Release-iphoneos/Unity-iPhone.build/Script-F6E34F12805380276D47C8E3.sh
//
// 詳しくはこちらのスレッドを参照
// https://forum.unity.com/threads/ios-build-is-failing-seems-like-a-fastlane-problem-not-sure-how-to-proceed.682201/
//
// Issue
// https://issuetracker.unity3d.com/issues/usym-upload-auth-token-is-thrown-in-xcode-when-the-project-is-built-in-batchmode-with-runtests-and-cloud-diagnostics-enabled?_ga=2.39982434.1359137443.1572392698-1458852553.1554601439
//
namespace UnityCICD.Editor.Build
{
    public class AddFakeUploadTokenPostprocessor : IPostprocessBuildWithReport
    {
        public int callbackOrder => 100;

        public void OnPostprocessBuild(BuildReport report)
        {
            var pathToBuiltProject = report.summary.outputPath;
            var target = report.summary.platform;
            // Debug.LogFormat("Postprocessing build at \"{0}\" for target {1}", pathToBuiltProject, target);
            if (target != BuildTarget.iOS) return;

            PBXProject proj = new PBXProject();
            string pbxFilename = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
            proj.ReadFromFile(pbxFilename);

#if UNITY_2019_3_OR_NEWER
            string guid = proj.GetUnityMainTargetGuid();
#else
            string targetName = PBXProject.GetUnityTargetName();
            string guid = proj.TargetGuidByName(targetName);
#endif

            proj.SetBuildProperty(guid, "USYM_UPLOAD_AUTH_TOKEN", "FakeToken");

            proj.WriteToFile(pbxFilename);
        }
    }
}
#endif