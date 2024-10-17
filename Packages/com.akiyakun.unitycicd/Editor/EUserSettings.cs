using UnityEngine;
using UnityEditor;
using System.Runtime.CompilerServices;

namespace unicicd.Editor
{
    // UnityEditor.EditorUserSettings のラッパークラス
    // UnityEditor のnamespaceと重複するのでEUserSettingsとしています。
    [System.Serializable]
    public class EUserSettings
    {
        public string Prefix { get; set; } = "unicicd_";

        public EUserSettings()
        {
        }

        public EUserSettings(string prefix)
        {
            Prefix = prefix;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetConfigValue(string name)
        {
            return EditorUserSettings.GetConfigValue($"{Prefix}{name}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetConfigValue(string name, string value)
        {
            EditorUserSettings.SetConfigValue($"{Prefix}{name}", value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetConfigString(string name, string defaultValue = null)
        {
            var ret = GetConfigValue(name);
            if (string.IsNullOrEmpty(ret) && defaultValue != null) return defaultValue;
            return ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetConfigString(string name, string value)
        {
            SetConfigValue(name, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetConfigInt(string name, int defaultValue = 0)
        {
            var value = GetConfigValue(name);
            int ret = defaultValue;
            if (string.IsNullOrEmpty(value)) return ret;

            try
            {
                ret = int.Parse(value);
            }
            catch
            {
                Debug.LogError($"Failed to parse int value: {value}");
                ret = defaultValue;
            }

            return ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetConfigInt(string name, int value)
        {
            SetConfigValue(name, value.ToString());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GetConfigBool(string name, bool defaultValue)
        {
            var ret = GetConfigValue(name);
            if (string.IsNullOrEmpty(ret)) return defaultValue;
            return ret == "True";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetConfigBool(string name, bool value)
        {
            SetConfigValue(name, value ? "True" : "False");
        }

    }
}

