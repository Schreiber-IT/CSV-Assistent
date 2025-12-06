using System;
using System.Collections.Generic;
using System.Text;

namespace CSVAssistent.Services.Settings
{
    public interface ISettingsService
    {
        T Get<T>(string key, T defaultValue = default!);
        void Set<T>(string key, T value);

        bool Exists(string key);
        void Remove(string key);
    }

}
