using System;
using System.Collections.Generic;
using System.Text;

namespace CSVAssistent.Services.Settings
{
    public class SettingEntry
    {
        [LiteDB.BsonId]
        public string Key { get; set; } = string.Empty;

        public LiteDB.BsonValue Value { get; set; } = LiteDB.BsonValue.Null;

        // Optional, falls du später auswerten willst:
        public string? TypeName { get; set; }
    }

}
