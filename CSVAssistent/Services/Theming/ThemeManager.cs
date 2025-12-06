using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CSVAssistent.Services.Theming
{
    public static class ThemeManager
    {
        private static ResourceDictionary? _currentTheme;

        // Hilfsfunktion: Theme-Dictionary laden (ohne es direkt zu aktivieren)
        private static ResourceDictionary LoadTheme(string uri)
            => new ResourceDictionary { Source = new Uri(uri, UriKind.Relative) };

        // Den aktuell aktiven Theme-Dict finden (per Marker)
        private static ResourceDictionary? FindCurrentTheme()
        {
            foreach (var dict in System.Windows.Application.Current.Resources.MergedDictionaries)
            {
                if (dict.Contains("Theme.Id"))
                    return dict;
            }
            return null;
        }

        public static string GetThemeID()
        {
            return CurrentId() ?? "Dark";
        }

        public static void Apply(string themeUri)
        {
            var app = System.Windows.Application.Current;
            var dicts = app.Resources.MergedDictionaries;

            // aktuelles Theme herausfinden
            _currentTheme ??= FindCurrentTheme();

            // neues Theme laden
            var newTheme = LoadTheme(themeUri);

            // altes Theme entfernen (falls vorhanden)
            if (_currentTheme != null)
                dicts.Remove(_currentTheme);

            // neues Theme hinzufügen und merken
            dicts.Insert(0, newTheme); // gerne an Position 0
            _currentTheme = newTheme;
        }

        public static string? CurrentId()
            => _currentTheme != null && _currentTheme.Contains("Theme.Id")
                ? _currentTheme["Theme.Id"] as string
                : FindCurrentTheme()?["Theme.Id"] as string;
    }

}
