namespace CSVAssistent.Models
{
    public class ColumnOption
    {
        public ColumnOption(int index, string name)
        {
            Index = index;
            Name = name;
        }

        public int Index { get; }
        public string Name { get; }
    }
}
