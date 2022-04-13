using Newtonsoft.Json;

namespace DriveShare.Helpers;

/// <summary>
/// A full result, as understood by jQuery DataTables.
/// </summary>
public class Result<T>
{
    [JsonProperty("draw")]
    public int Draw { get; set; }
    [JsonProperty("recordsTotal")]
    public int RecordsTotal { get; set; }
    [JsonProperty("recordsFiltered")]
    public int RecordsFiltered { get; set; }
    [JsonProperty("data")]
    public IEnumerable<T> Data { get; set; }
    [JsonProperty("error", NullValueHandling = NullValueHandling.Ignore)]
    public string Error { get; set; }
    public string PartialView { get; set; }
}

/// <summary>
/// The additional columns that you can send to jQuery DataTables for automatic processing.
/// </summary>
public abstract class DtRow
{
    [JsonProperty("DT_RowId")]
    public virtual string DtRowId => null;

    [JsonProperty("DT_RowClass")]
    public virtual string DtRowClass => null;

    [JsonProperty("DT_RowData")]
    public virtual object DtRowData => null;

    [JsonProperty("DT_RowAttr")]
    public virtual object DtRowAttr => null;
}

public class DtParameters
{
    public int Draw { get; set; }
    public DtColumn[] Columns { get; set; }
    public DtOrder[] Order { get; set; }
    public int Start { get; set; }
    public int Length { get; set; }
    public DtSearch Search { get; set; }
    public string SortOrder => Columns != null && Order != null && Order.Length > 0
        ? (Columns[Order[0].Column].Data +
           (Order[0].Dir == DtOrderDir.Desc ? " " + Order[0].Dir : string.Empty))
        : null;
    public IEnumerable<string> AdditionalValues { get; set; }

}

public class DtColumn
{
    public string Data { get; set; }
    public string Name { get; set; }

    //Flag to indicate if this column is searchable (true) or not (false). This is controlled by columns.searchable.
    public bool Searchable { get; set; }

    //Flag to indicate if this column is orderable (true) or not (false). This is controlled by columns.orderable.
    public bool Orderable { get; set; }

    //Search value to apply to this specific column.
    public DtSearch Search { get; set; }
}

public class DtOrder
{
    public int Column { get; set; }

    public DtOrderDir Dir { get; set; }
}

public enum DtOrderDir
{
    Asc,
    Desc
}

public class DtSearch
{
    public string Value { get; set; }
    public bool Regex { get; set; }
}