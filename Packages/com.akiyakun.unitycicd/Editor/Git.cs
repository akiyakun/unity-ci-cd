using System;
using UnityEngine;
using System.IO;
using System.Text;
using UnityEditor;

namespace unicicd.Editor
{
    public class Git
    {
        static string Call(string arg, bool removeNewLine = true)
        {
            string ret = BuildUtility.DoConsoleCommand("git", arg);

            if (removeNewLine)
            {
                // ret = ret.Replace(Environment.NewLine, "");
                ret = ret.Replace("\n", "");
                ret = ret.Replace("\r", "");
            }

            return ret;
        }

        public static string GetCommitInfo()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("[CommitInfo]\n");

            // ビルド日時
            {
                var dateTime = DateTime.Now;
                sb.AppendFormat(
                    // 秒まであり
                    // "BuildDateTime: {0:D4}/{1:D2}/{2:D2} {3:D2}:{4:D2}:{5:D2}\n",
                    // dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second

                    "Date: {0:D4}/{1:D2}/{2:D2} {3:D2}:{4:D2}",
                    dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute
                );
            }

            sb.Append(Environment.NewLine);

            try
            {
                sb.Append($"branch - hash: {Call("rev-parse --abbrev-ref HEAD")} - {Call("rev-parse --short HEAD")}");
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
            }

            return sb.ToString();
        }

        public static void WriteBuildInfoText()
        {
            var config = CICDConfig.Load();
            File.WriteAllText(
                config.BuildSettings.AdditionalInfoSettings.BuildInfoTextPath,
                GetCommitInfo()
            );
            AssetDatabase.Refresh();
        }

        public static void DeleteBuildInfoText()
        {
            var config = CICDConfig.Load();
            File.Delete(config.BuildSettings.AdditionalInfoSettings.BuildInfoTextPath);
            AssetDatabase.Refresh();
        }

        // デバッグテスト用
#if false
        [UnityEditor.MenuItem("App/GitTest/GetCommitInfo")]
        public static void Test_GetCommitInfo()
        {
            EditorUtility.DisplayDialog("GetCommitInfo()", GetCommitInfo(), "OK");
        }

        [UnityEditor.MenuItem("App/GitTest/WriteBuildInfoText")]
        public static void Test_WriteBuildInfoText() => WriteBuildInfoText();

        [UnityEditor.MenuItem("App/GitTest/DeleteBuildInfoText")]
        public static void Test_DeleteBuildInfoText() => DeleteBuildInfoText();
#endif
    }
}
