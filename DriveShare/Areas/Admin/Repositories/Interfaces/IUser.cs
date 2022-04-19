using DriveShare.Areas.Admin.Enums;
using DriveShare.Areas.Admin.ViewModels;
using DriveShare.Areas.Admin.Models;

namespace DriveShare.Areas.Admin.Repositories.Interfaces;
public interface IUser
{
    Task<IEnumerable<UserViewModel>> GetUsersAsync(UserStatus userStatus);
    Task<IEnumerable<UserViewModel>> SearchAsync(string term);
    Task<int> GetUsersCountAsync(int month);
    Task<OperationResult> ChangeUserStatusAsync(string id, UserStatus userStatus);
}