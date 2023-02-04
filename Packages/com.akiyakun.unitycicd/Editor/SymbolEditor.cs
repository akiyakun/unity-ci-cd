using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace unicicd.Editor
{
    public class SymbolEditor
    {
        public static void AddSymbol(string define, BuildTargetGroup targetGroup = BuildTargetGroup.Unknown)
        {
            var group = targetGroup == BuildTargetGroup.Unknown ? EditorUserBuildSettings.selectedBuildTargetGroup : targetGroup;
            var symbols = GetSymbols(group);

            if (symbols.Contains(define))
            {
                return;
            }

            symbols.Add(define);
            SetSymbols(symbols, group);
        }

        public static void RemoveSymbol(string define, BuildTargetGroup targetGroup = BuildTargetGroup.Unknown)
        {
            var group = targetGroup == BuildTargetGroup.Unknown ? EditorUserBuildSettings.selectedBuildTargetGroup : targetGroup;
            var symbols = GetSymbols(group);
            symbols.Remove(define);
            SetSymbols(symbols, group);
        }

        public static List<string> GetSymbols(BuildTargetGroup targetGroup = BuildTargetGroup.Unknown)
        {
            var group = targetGroup == BuildTargetGroup.Unknown ? EditorUserBuildSettings.selectedBuildTargetGroup : targetGroup;
            return new List<string>(PlayerSettings.GetScriptingDefineSymbolsForGroup(group).Split(new[] { ';' }));
        }

        public static void SetSymbols(List<string> defineList, BuildTargetGroup targetGroup = BuildTargetGroup.Unknown)
        {
            var group = targetGroup == BuildTargetGroup.Unknown ? EditorUserBuildSettings.selectedBuildTargetGroup : targetGroup;

            string defines = "";
            foreach (string d in defineList)
            {
                defines += ";" + d;
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, defines);
        }
    }
}
