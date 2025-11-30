using System.Collections.Generic;

namespace GameSettings
{
    public static class CustomGameSettings
    {
        private static Dictionary<string, object> _settings = new Dictionary<string, object>();

        public static void SaveSetting(string key, object value)
        {
            if (_settings.ContainsKey(key))
                _settings[key] = value;
            else
                _settings.Add(key, value);
        }

        public static object GetSetting(string key)
        {
            if (!_settings.ContainsKey(key))
                return null;

            return _settings[key];
        }

        public static string GetStringSetting(string key)
        {
            object setting = GetSetting(key);
            if (setting != null)
                return (string)setting;

            return null;
        }

        public static int GetIntSetting(string key)
        {
            object setting = GetSetting(key);
            if (setting != null)
                return (int)setting;

            return 0;
        }

        public static float GetFloatSetting(string key)
        {
            object setting = GetSetting(key);
            if (setting != null)
                return (float)setting;

            return 0f;
        }
    }
}
