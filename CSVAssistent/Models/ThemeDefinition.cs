using System;
using System.Collections.Generic;
using System.Text;

namespace CSVAssistent.Models
{
    public class ThemeDefinition
    {
        public string Id { get; }
        public string DisplayName { get; }
        public string ResourceUri { get; }

        public ThemeDefinition(string id, string displayName, string resourceUri)
        {
            Id = id;
            DisplayName = displayName;
            ResourceUri = resourceUri;
        }

        public override string ToString() => DisplayName;
    }
}
