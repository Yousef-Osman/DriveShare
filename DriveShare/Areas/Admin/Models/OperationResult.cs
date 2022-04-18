namespace DriveShare.Areas.Admin.Models;

public class OperationResult
{
    public bool Success { get; set; }
    public string Message { get; set; }

    public static OperationResult NotFound(string message = "UserNotFound")
    {
        return new OperationResult()
        {
            Success = false,
            Message = message
        };
    }

    public static OperationResult Succeeded(string message = "Operation completed successfully")
    {
        return new OperationResult()
        {
            Success = true,
            Message = message
        };
    }
}
