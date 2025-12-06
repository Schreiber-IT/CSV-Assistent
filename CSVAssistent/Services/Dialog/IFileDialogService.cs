using System;
using System.Collections.Generic;
using System.Text;

namespace CSVAssistent.Services.Dialog
{
    public interface IFileDialogService
    {
        string? OpenFile(string filter = "CSV Dateien (*.csv)|*.csv|Alle Dateien (*.*)|*.*");
        string[]? OpenFiles(string filter = "CSV Dateien (*.csv)|*.csv|Alle Dateien (*.*)|*.*");
        string? OpenFolder();

        string? SaveFile(string filter = "JSON Dateien (*.json)|*.json|Alle Dateien (*.*)|*.*", string defaultExt = "json");

        string? SaveFileAs(string filter = "CSV Dateien (*.csv)|*.csv|Alle Dateien (*.*)|*.*", string defaultExt = "csv");
    }
}
