using System;
using System.Collections.Generic;
using System.Text;

namespace CSVAssistent.Services.Settings
{
    public static class SettingsExtensions
    {
        public static string GetString(this ISettingsService settings, string key, string defaultValue = "")
            => settings.Get<string>(key, defaultValue);

        public static int GetInt(this ISettingsService settings, string key, int defaultValue = 0)
            => settings.Get<int>(key, defaultValue);

        public static bool GetBool(this ISettingsService settings, string key, bool defaultValue = false)
            => settings.Get<bool>(key, defaultValue);

        public static void SetString(this ISettingsService settings, string key, string value)
            => settings.Set<string>(key, value);

        public static void SetInt(this ISettingsService settings, string key, int value)
            => settings.Set<int>(key, value);

        public static void SetBool(this ISettingsService settings, string key, bool value)
            => settings.Set<bool>(key, value);
    }

}
