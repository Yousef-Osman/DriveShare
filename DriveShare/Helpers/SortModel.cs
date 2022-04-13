using DriveShare.Models.Enums;

namespace DriveShare.Helpers
{
    public class SortModel
    {
        public string SortedProperty { get; set; }
        public SortOrder SortedOrder { get; set; }
        private List<SortableColumn> sortableColumns = new ();

        public void AddColumn(string columnName)
        {
            var column = sortableColumns.Where (c => c.ColumnName.ToLower() == columnName.ToLower()).SingleOrDefault ();

            if(column == null)
                sortableColumns.Add(column);
        }

        public SortableColumn GetColumn(string columnName)
        {
            var column = sortableColumns.Where(c => c.ColumnName.ToLower() == columnName.ToLower()).SingleOrDefault();

            if (column == null) 
                sortableColumns.Add(column);

            return column;
        }
    }

    public class SortableColumn
    {
        public string ColumnName { get; set; }
        public string SOrtExpression { get; set; }
        public string SortIcon { get; set; }
    }
}
