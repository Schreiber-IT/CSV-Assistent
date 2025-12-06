using System;
using System.Collections.Generic;
using System.Text;
using CSVAssistent.Models;

namespace CSVAssistent.Services.Theming
{
    public class ThemeService : IThemeService
    {
        public IReadOnlyList<ThemeDefinition> AvailableThemes { get; }

        public string CurrentThemeId => ThemeManager.GetThemeID();

        public ThemeService()
        {
            // Themes hier einmal definieren
            AvailableThemes = new[]
            {
                new ThemeDefinition("Dark",    "Dunkel",   "Resources/Themes/Dark.xaml"),
                new ThemeDefinition("Light",   "Hell",     "Resources/Themes/Light.xaml"),
                new ThemeDefinition("MidLight","Mid-Light","Resources/Themes/MidLight.xaml"),
            };
        }

        public void ApplyTheme(string themeId)
        {
            var theme = AvailableThemes.FirstOrDefault(t => t.Id == themeId);
            if (theme == null)
                return;

            ThemeManager.Apply(theme.ResourceUri);

            // Optional: Theme in Settings speichern
            // Settings.Default.ThemeId = themeId;
            // Settings.Default.Save();
        }
    }

}
