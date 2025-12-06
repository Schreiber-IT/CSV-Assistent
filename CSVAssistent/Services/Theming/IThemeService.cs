using System;
using System.Collections.Generic;
using System.Text;
using CSVAssistent.Models;

namespace CSVAssistent.Services.Theming
{
    public interface IThemeService
    {
        IReadOnlyList<ThemeDefinition> AvailableThemes { get; }
        string CurrentThemeId { get; }

        void ApplyTheme(string themeId);
    }
}
