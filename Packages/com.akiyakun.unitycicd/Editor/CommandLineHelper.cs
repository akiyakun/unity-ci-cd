using System;
using System.Collections.Generic;
using UnityEditor;

namespace unicicd.Editor
{
    public static class CommandLineHelper
    {
        static List<string> additiveArgs;

        // システムのコマンドライン引数とこのクラスが内部で保持しているコマンドライン引数をマージして返します
        public static List<string> GetCommandLineArgs()
        {
            List<string> ret = new(System.Environment.GetCommandLineArgs());
            
            if (additiveArgs != null)
            {
                ret.AddRange(additiveArgs);
            }

            return ret;
        }

        public static void AddCommandLineArgs(params string[] args)
        {
            if (additiveArgs == null)
            {
                additiveArgs = new List<string>();
            }

            foreach (var argument in args)
            {
                additiveArgs.Add(argument);
            }

        }

    }
}
