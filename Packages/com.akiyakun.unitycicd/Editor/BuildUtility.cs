using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace unicicd.Editor
{
    public class BuildUtility
    {
        #region Path
        /// <summary>
        /// プロジェクトルートパスを取得
        /// 末尾に"/"が付きます。
        /// </summary>
        public static string GetRootPath()
        {
            return Path.GetDirectoryName(Application.dataPath) + "/";
        }

        /// <summary>
        /// パス文字列の正規化
		/// "\\" > "/"
        /// </summary>
        public static string GetPathNormalization(string path)
        {
            return path.Replace("\\", "/");
        }

        /// <summary>
        /// パスの結合
        /// https://sassembla.github.io/Public/2015:02:24%200-32-46/2015:02:24%200-32-46.html
        /// https://light11.hatenadiary.com/entry/2021/09/21/200054
        /// </summary>
        public static string PathCombine(string path1, string path2, bool normalization = false)
        {
            if (normalization)
            {
                path1 = GetPathNormalization(path1);
                path2 = GetPathNormalization(path2);
            }

            path1 = path1.TrimEnd('/');

            if (path2.StartsWith('/'))
            {
                return path1 + path2;
            }
            else
            {
                return path1 + "/" + path2;
            }
        }
        #endregion

        #region File
        /// <summary>
        /// ファイルの削除
        /// 内部でFile.Exists()チェックしているので存在しないファイルを指定してもエラーになりません。
        /// </summary>
        public static void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        /// <summary>
        /// ファイルの削除
        /// .metaファイルも一緒に削除します。
        /// </summary>
        public static void DeleteFileAndMeta(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            path = Path.GetFileNameWithoutExtension(path) + ".meta";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public static void CreateDirectory(string path)
        {
            bool isDirectory = Path.GetExtension(path).Length == 0;
            if (!isDirectory)
            {
                path = Path.GetDirectoryName(path);
            }

            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch
                {
                    Debug.Assert(false, "Error is CreateDirectory():" + path);
                }
            }
        }

        public static void DeleteDirectory(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
            }
            catch
            {
                Debug.Log("DeleteDirectory Failed: " + path);
            }
        }

        // https://docs.microsoft.com/ja-jp/dotnet/standard/io/how-to-copy-directories
        public static void DirectoryCopy(string sourceDirName, string destDirName, bool overwrite)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            CreateDirectory(destDirName);

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                // Debug.Log(temppath);
                file.CopyTo(temppath, overwrite);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, temppath, overwrite);
            }
        }
        #endregion

        #region UnitySettings
        // ビルドセッティングスに設定してあるシーンを取得
        public static List<string> GetBuildSettingsScene()
        {
            List<string> scenes = new List<string>();

            foreach (var s in EditorBuildSettings.scenes)
            {
                if (s.enabled)
                {
                    scenes.Add(s.path);
                }
            }

            return scenes;
        }
        #endregion

        #region Execute
        public static string DoConsoleCommand(string cmd, string arg = "", string workDir = "")
        {
            // Debug.Log("WorkDir: " + workDir);
            // Debug.Log("Command: " + cmd);

            var process = new System.Diagnostics.Process();
            if (workDir == null) workDir = GetRootPath();
            process.StartInfo.WorkingDirectory = workDir;

            process.StartInfo.FileName = cmd;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.Arguments = arg;
            process.StartInfo.CreateNoWindow = true;

            string output = "";

            try
            {
                process.Start();
                output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
            }
            finally
            {
                process.Close();
            }

            return output;
        }
        /*
        public static int DoConsoleCommand(string cmd, string workDir = "")
        {
            Debug.Log("WorkDir: " + workDir);
            Debug.Log("Command: " + cmd);

            var process = new System.Diagnostics.Process();
            if (workDir == null) workDir = GetRootPath();
            process.StartInfo.WorkingDirectory = workDir;

            process.StartInfo.FileName = "/bin/bash";
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.Arguments = "-c \" "
            + "source ~/.bashrc; "
            + cmd
            + " \"";

            string output;
            // if (!process.Start())
            //     output = process.StandardError.ReadToEnd();
            // else
            //     output = process.StandardOutput.ReadToEnd();
            try
            {
                process.Start();
                process.WaitForExit();
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
            }

            output = process.StandardOutput.ReadToEnd();
            output += process.StandardError.ReadToEnd();
            Debug.Log(output);

            int exitCode = process.ExitCode;

            process.Close();

            return exitCode;
        }
        //*/
        #endregion
    }
}