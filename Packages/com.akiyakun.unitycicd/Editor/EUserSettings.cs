using UnityEditor;
using System.Runtime.CompilerServices;

namespace unicicd.Editor
{
    // UnityEditor.EditorUserSettings のラッパークラス
    // UnityEditor のnamespaceと重複するのでEUserSettingsとしています。
    [System.Serializable]
    public static class EUserSettings
    {
        const string Prefix = "unicicd_";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static string GetConfigValue(string name)
        {
            return EditorUserSettings.GetConfigValue($"{Prefix}{name}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void SetConfigValue(string name, string value)
        {
            EditorUserSettings.SetConfigValue($"{Prefix}{name}", value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetConfigString(string name, string defaultValue = null)
        {
            var ret = GetConfigValue(name);
            if (string.IsNullOrEmpty(ret) && defaultValue != null) return defaultValue;
            return ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetConfigString(string name, string value)
        {
            SetConfigValue(name, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetConfigInt(string name, int defaultValue = 0)
        {
            var ret = GetConfigValue(name);
            if (string.IsNullOrEmpty(ret)) return defaultValue;
            return int.Parse(ret);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetConfigInt(string name, int value)
        {
            SetConfigValue(name, value.ToString());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GetConfigBool(string name, bool defaultValue)
        {
            var ret = GetConfigValue(name);
            if (string.IsNullOrEmpty(ret)) return defaultValue;
            return ret == "True";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetConfigBool(string name, bool value)
        {
            SetConfigValue(name, value ? "True" : "False");
        }

    }
}

