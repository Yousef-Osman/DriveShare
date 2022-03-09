namespace DriveShare.Models;

public class BaseClass
{
    public BaseClass()
    {
        Id = Guid.NewGuid().ToString();
        CreatedOn = DateTime.Now;
    }

    public string Id { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime LastModifiedOn { get; set; }
    public DateTime DeletedOn { get; set; }
}
