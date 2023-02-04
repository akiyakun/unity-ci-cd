using System;
using System.Collections.Generic;
using UnityEditor;
using unicicd.Editor;

namespace unicicd.Editor.Build
{
    /*
        ビルド用のコマンドラインを処理するクラス
     */
    public class CommandLineBuild
    {
        public static void Build()
        {
            string platform = null;
            bool development = false;
            string[] options = null;

            // 引数をパース
            string[] args = System.Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; ++i)
            {
                switch (args[i])
                {
                    case "--platform":
                        platform = args[i + 1];
                        break;
                    case "--development":
                        development = true;
                        break;
                    case "--options":
                        options = args[i + 1].Split(',');
                        break;
                }
            }

            Console.WriteLine(platform);

            BuildTarget buildTarget;
            if (!Enum.TryParse<BuildTarget>(platform, out buildTarget))
            {
                Console.WriteLine("[Build] Unknown platform is " + platform);
                EditorApplication.Exit(0);
            }


            CICDBuilder builder = new CICDBuilder();
            var ret = builder.Build(
                development: development,
                jobName: "",
                optionStrings: options,
                buildTarget: buildTarget
            );

            if (ret.BuildSucceeded)
            {
                EditorApplication.Exit(0);
            }
            else
            {
                EditorApplication.Exit(1);
            }
        }
    }
}
