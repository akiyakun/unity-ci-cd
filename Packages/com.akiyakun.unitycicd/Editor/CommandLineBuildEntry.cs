using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using unicicd.Editor;

#pragma warning disable CS0219

// namespace unicicd.Editor
// {
/*
    ビルド用のコマンドラインを処理するクラス
 */
// public class CommandLineBuild
public static class CI
{
    public static bool EditorDebug = false;

    public static void Build()
    {
        try
        {
            _Build();
        }
        catch (Exception e)
        {
            Print(e.Message);
            Exit(1);
        }
    }

    static void _Build()
    {
        CICDBuildOptions buildOptions = new CICDBuildOptions();
        buildOptions.SetupDefaultSettings();

        string[] optionStrings = null;

        // 引数をパース
        var args = CommandLineHelper.GetCommandLineArgs();
        for (int i = 0; i < args.Count; ++i)
        {
            switch (args[i])
            {
                case "--platform":
                    {
                        var platform = args[i + 1];
                        if (!Enum.TryParse<BuildTarget>(platform, out buildOptions.BuildTarget))
                        {
                            throw new Exception("Unknown platform is " + platform);
                        }
                        Print($"--platform {buildOptions.BuildTarget.ToString()}");
                    }
                    break;
                case "--buildmode":
                    {
                        var buildmode = args[i + 1];
                        if (!Enum.TryParse<CICDBuildMode>(buildmode, out buildOptions.BuildMode))
                        {
                            throw new Exception("Unknown buildmode is " + buildmode);
                        }
                        Print($"--buildmode {buildOptions.BuildMode.ToString()}");
                    }
                    break;
                case "--development":
                    buildOptions.UnityDevelopmentBuild = true;
                    break;
                case "--options":
                    optionStrings = args[i + 1].Split(',');
                    break;
                case "--inappdebug":
                    buildOptions.InAppDebug = true;
                    break;
            }
        }

        CICDBuilder builder = new CICDBuilder();
        // options.BuildMode = CICDBuildMode.Debug;
        // buildOptions.BuildTarget = buildTarget;
        // buildOptions.BuildMode = CICDBuildMode.Debug;
        // buildOptions.InAppDebug = true;
        // buildOptions.OptionStrings.AddRange(optionStrings);

        foreach (var option in optionStrings ?? new string[0])
        {
            buildOptions.Options.Add(option, null);
        }

        if (builder.Initialize(buildOptions) == false)
        {
            Exit(1);
            return;
        }

        var ret = builder.Build();
        if (ret.BuildSucceeded)
        {
            Exit(0);
        }
        else
        {
            Exit(1);
        }
    }

    static void Print(string msg)
    {
        if (EditorDebug)
        {
            Debug.Log(msg);
            return;
        }

        Console.WriteLine(msg);
    }

    static void Exit(int exitCode)
    {
        if (EditorDebug)
        {
            if (exitCode == 0)
            {
                Debug.Log($"ExitCode={exitCode}");
            }
            else
            {
                Debug.LogError($"Error: ExitCodd={exitCode}");
            }
            return;
        }

        EditorApplication.Exit(exitCode);
    }

}
// }
