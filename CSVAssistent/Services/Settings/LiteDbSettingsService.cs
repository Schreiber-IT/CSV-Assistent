using System;
using System.Collections.Generic;
using System.Text;
using LiteDB;

namespace CSVAssistent.Services.Settings
{
    public class LiteDbSettingsService : ISettingsService, IDisposable
    {
        private readonly LiteDatabase _db;
        private readonly ILiteCollection<SettingEntry> _collection;

        public LiteDbSettingsService(string dbPath)
        {
            _db = new LiteDatabase(dbPath);
            _collection = _db.GetCollection<SettingEntry>("settings");
        }

        public T Get<T>(string key, T defaultValue = default!)
        {
            var entry = _collection.FindById(key);
            if (entry == null || entry.Value.IsNull)
                return defaultValue;

            try
            {
                // Versuch, direkt in T zu casten
                var bson = entry.Value;

                // Einfache Fälle: int, string, bool, double, DateTime...
                var result = bson.RawValue;

                if (result is T t)
                    return t;

                // Fallback über Convert.ChangeType für primitive Typen
                if (result != null)
                {
                    return (T)Convert.ChangeType(result, typeof(T));
                }
            }
            catch
            {
                // Wenn etwas schiefgeht, Default zurückgeben
            }

            return defaultValue;
        }

        public void Set<T>(string key, T value)
        {
            var bsonValue = new BsonValue(value);

            var entry = new SettingEntry
            {
                Key = key,
                Value = bsonValue,
                TypeName = typeof(T).FullName
            };

            _collection.Upsert(entry);
        }

        public bool Exists(string key)
        {
            return _collection.Exists(e => e.Key == key);
        }

        public void Remove(string key)
        {
            _collection.Delete(key);
        }

        public void Dispose()
        {
            _db?.Dispose();
        }
    }

}
