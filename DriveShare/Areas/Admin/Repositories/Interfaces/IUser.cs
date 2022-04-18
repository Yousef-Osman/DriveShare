using DriveShare.Areas.Admin.Enums;
using DriveShare.Areas.Admin.ViewModels;
using DriveShare.Areas.Admin.Models;

namespace DriveShare.Areas.Admin.Repositories.Interfaces;
public interface IUser
{
    Task<IEnumerable<UserViewModel>> GetUsers(UserStatus userStatus);
    Task<IEnumerable<UserViewModel>> Search(string term);
    Task<int> GetUsersCount(int month);
    Task<OperationResult> ChangeUserStatus(string id, UserStatus userStatus);
}