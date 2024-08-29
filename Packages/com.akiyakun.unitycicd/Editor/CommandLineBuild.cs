using System;
using System.Collections.Generic;
using UnityEditor;
using unicicd.Editor;

#pragma warning disable CS0219

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
            string[] optionStrings = null;

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
                        optionStrings = args[i + 1].Split(',');
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
            CICDBuildOptions options = new CICDBuildOptions();
            options.Build = CICDBuildOptions.BuildMode.Debug;
            options.BuildTarget = buildTarget;
            options.OptionStrings = new List<string>(optionStrings);

            if (builder.Initialize(options) == false)
            {
                EditorApplication.Exit(1);
            }

            var ret = builder.Build();
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
