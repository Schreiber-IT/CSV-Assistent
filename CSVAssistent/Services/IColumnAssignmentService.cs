using System.Collections.ObjectModel;
using CSVAssistent.Models;

namespace CSVAssistent.Services
{
    public interface IColumnAssignmentService
    {
        ObservableCollection<ColumnAssignment> Mappings { get; }
        ColumnAssignment AddMapping(string targetColumn);

        /// <summary>
        /// Sets the file context for column assignments so that source columns are stored per file.
        /// </summary>
        /// <param name="fileName">Full path or unique name of the file.</param>
        void SetCurrentFile(string fileName);

        /// <summary>
        /// Speichert zentrale Spalten und zugehörige Zuordnungen in eine JSON-Datei.
        /// </summary>
        /// <param name="filePath">Zielpfad der JSON-Datei.</param>
        void SaveToFile(string filePath);

        /// <summary>
        /// Lädt zentrale Spalten und Zuordnungen aus einer JSON-Datei.
        /// </summary>
        /// <param name="filePath">Pfad zur JSON-Datei.</param>
        void LoadFromFile(string filePath);

        /// <summary>
        /// Prüft, ob für die angegebene Datei mindestens eine Spaltenzuordnung existiert.
        /// </summary>
        /// <param name="fileName">Dateiname oder Pfad der Datei.</param>
        /// <returns><c>true</c>, wenn mindestens eine Zuordnung existiert, andernfalls <c>false</c>.</returns>
        bool HasAnyMapping(string fileName);
    }
}
