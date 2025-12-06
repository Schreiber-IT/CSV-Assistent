using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using CSVAssistent.Models;
using System.IO;
using System.Text.Json;

namespace CSVAssistent.Services
{
    public class ColumnAssignmentService : IColumnAssignmentService
    {
        public ObservableCollection<ColumnAssignment> Mappings { get; } = new();

        private readonly Dictionary<string, Dictionary<string, int?>> _fileAssignments =
            new(StringComparer.OrdinalIgnoreCase);

        private string? _currentFile;

        public ColumnAssignmentService()
        {
            Mappings.CollectionChanged += OnMappingsCollectionChanged;
        }

        private class AssignmentFile
        {
            public List<MappingFileEntry> Mappings { get; set; } = new();
            public Dictionary<string, Dictionary<string, int?>> FileAssignments { get; set; } =
                new(StringComparer.OrdinalIgnoreCase);
        }

        private class MappingFileEntry
        {
            public string TargetColumn { get; set; } = string.Empty;
        }

        public void SetCurrentFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return;
            }

            _currentFile = fileName;
            ApplyAssignmentsForCurrentFile();
        }

        public ColumnAssignment AddMapping(string targetColumn)
        {
            if (string.IsNullOrWhiteSpace(targetColumn))
                throw new ArgumentException("Target column must not be empty.", nameof(targetColumn));

            var existing = Mappings.FirstOrDefault(m =>
                string.Equals(m.TargetColumn, targetColumn, StringComparison.OrdinalIgnoreCase));

            if (existing != null)
                return existing;

            var newMapping = new ColumnAssignment
            {
                TargetColumn = targetColumn.Trim()
            };
            Mappings.Add(newMapping);
            return newMapping;
        }

        public void SaveToFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path must not be empty.", nameof(filePath));

            var data = new AssignmentFile
            {
                Mappings = Mappings
                    .Select(m => new MappingFileEntry { TargetColumn = m.TargetColumn })
                    .ToList(),
                FileAssignments = _fileAssignments.ToDictionary(
                    pair => pair.Key,
                    pair => new Dictionary<string, int?>(pair.Value, StringComparer.OrdinalIgnoreCase),
                    StringComparer.OrdinalIgnoreCase)
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(data, options);
            File.WriteAllText(filePath, json);
        }

        public void LoadFromFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path must not be empty.", nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException("Assignment file not found", filePath);

            var json = File.ReadAllText(filePath);
            var data = JsonSerializer.Deserialize<AssignmentFile>(json) ?? new AssignmentFile();

            _fileAssignments.Clear();
            foreach (var fileEntry in data.FileAssignments)
            {
                _fileAssignments[fileEntry.Key] = new Dictionary<string, int?>(fileEntry.Value,
                    StringComparer.OrdinalIgnoreCase);
            }

            foreach (var mapping in Mappings)
            {
                mapping.PropertyChanged -= OnMappingPropertyChanged;
            }

            Mappings.Clear();

            foreach (var mappingEntry in data.Mappings)
            {
                var mapping = new ColumnAssignment { TargetColumn = mappingEntry.TargetColumn };
                Mappings.Add(mapping);
            }

            ApplyAssignmentsForCurrentFile();
        }

        public bool HasAnyMapping(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return false;

            if (!_fileAssignments.TryGetValue(fileName, out var assignments))
                return false;

            return assignments.Values.Any(value => value.HasValue);
        }

        private void OnMappingsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems == null) return;

            foreach (var item in e.NewItems.OfType<ColumnAssignment>())
            {
                SubscribeToMapping(item);
                ApplySavedAssignment(item);
            }
        }

        private void SubscribeToMapping(ColumnAssignment mapping)
        {
            mapping.PropertyChanged -= OnMappingPropertyChanged;
            mapping.PropertyChanged += OnMappingPropertyChanged;
        }

        private void OnMappingPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is not ColumnAssignment mapping)
                return;

            if (e.PropertyName == nameof(ColumnAssignment.SourceColumnIndex))
            {
                SaveSourceColumn(mapping);
            }
        }

        private void SaveSourceColumn(ColumnAssignment mapping)
        {
            if (string.IsNullOrWhiteSpace(_currentFile))
                return;

            var assignments = GetAssignmentsForFile(_currentFile);

            if (!mapping.SourceColumnIndex.HasValue)
            {
                assignments.Remove(mapping.TargetColumn);
            }
            else
            {
                assignments[mapping.TargetColumn] = mapping.SourceColumnIndex;
            }
        }

        private void ApplyAssignmentsForCurrentFile()
        {
            if (string.IsNullOrWhiteSpace(_currentFile))
                return;

            var assignments = GetAssignmentsForFile(_currentFile);

            foreach (var mapping in Mappings)
            {
                if (assignments.TryGetValue(mapping.TargetColumn, out var source))
                {
                    mapping.SourceColumnIndex = source;
                }
                else
                {
                    mapping.SourceColumnIndex = null;
                }
            }
        }

        private void ApplySavedAssignment(ColumnAssignment mapping)
        {
            if (string.IsNullOrWhiteSpace(_currentFile))
                return;

            var assignments = GetAssignmentsForFile(_currentFile);

            if (assignments.TryGetValue(mapping.TargetColumn, out var source))
            {
                mapping.SourceColumnIndex = source;
            }
        }

        private Dictionary<string, int?> GetAssignmentsForFile(string fileName)
        {
            if (!_fileAssignments.TryGetValue(fileName, out var assignments))
            {
                assignments = new Dictionary<string, int?>(StringComparer.OrdinalIgnoreCase);
                _fileAssignments[fileName] = assignments;
            }

            return assignments;
        }
    }
}
