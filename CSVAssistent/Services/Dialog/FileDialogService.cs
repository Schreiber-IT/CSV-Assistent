using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSVAssistent.Services.Dialog
{
    public class FileDialogService : IFileDialogService
    {
        public string? OpenFile(string filter = "CSV Dateien (*.csv)|*.csv|Alle Dateien (*.*)|*.*")
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Filter = filter,
                Multiselect = false,
                CheckFileExists = true
            };
            return dlg.ShowDialog() == true ? dlg.FileName : null;
        }

        public string[]? OpenFiles(string filter = "CSV Dateien (*.csv)|*.csv|Alle Dateien (*.*)|*.*")
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Filter = filter,
                Multiselect = true,
                CheckFileExists = true
            };
            return dlg.ShowDialog() == true ? dlg.FileNames : null;
        }

        public string? OpenFolder()
        {
            using var dlg = new FolderBrowserDialog
            {
                ShowNewFolderButton = false,
                UseDescriptionForTitle = true,
                Description = "Ordner wählen"
            };
            return dlg.ShowDialog() == DialogResult.OK ? dlg.SelectedPath : null;
        }

        public string? SaveFile(string filter = "JSON Dateien (*.json)|*.json|Alle Dateien (*.*)|*.*", string defaultExt = "json")
        {
            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                Filter = filter,
                DefaultExt = defaultExt,
                AddExtension = true,
                OverwritePrompt = true
            };

            return dlg.ShowDialog() == true ? dlg.FileName : null;
        }

        public string? SaveFileAs(string filter = "CSV Dateien (*.csv)|*.csv|Alle Dateien (*.*)|*.*", string defaultExt = "csv")
        {
            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                Filter = filter,
                DefaultExt = defaultExt,
                AddExtension = true,
                OverwritePrompt = true
            };

            return dlg.ShowDialog() == true ? dlg.FileName : null;
        }
    }
}
