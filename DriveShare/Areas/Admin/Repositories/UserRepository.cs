using AutoMapper;
using AutoMapper.QueryableExtensions;
using DriveShare.Areas.Admin.Enums;
using DriveShare.Areas.Admin.Repositories.Interfaces;
using DriveShare.Areas.Admin.ViewModels;
using DriveShare.Areas.Admin.Models;
using DriveShare.Data;
using Microsoft.EntityFrameworkCore;

namespace DriveShare.Areas.Admin.Repositories;

public class UserRepository : IUser
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UserRepository(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserViewModel>> GetUsersAsync(UserStatus userStatus = 0) //if userStatus = 0 get all data
    {
        return await _context.Users.Where(a => (userStatus == 0) ? true : a.Status == (int)userStatus)
            .ProjectTo<UserViewModel>(_mapper.ConfigurationProvider).ToListAsync();
    }

    public async Task<IEnumerable<UserViewModel>> SearchAsync(string seearchValue)
    {
        return await _context.Users
            .Where(a => a.UserName == seearchValue || a.Email == seearchValue ||
                        a.FirstName == seearchValue || a.LastName == seearchValue)
            .ProjectTo<UserViewModel>(_mapper.ConfigurationProvider).ToListAsync();
    }

    public async Task<OperationResult> ChangeUserStatusAsync(string id, UserStatus userStatus)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
            return OperationResult.NotFound();

        user.Status = (int)userStatus;
        _context.Update(user);
        await _context.SaveChangesAsync();

        return OperationResult.Succeeded();
    }
    public async Task<int> GetUsersCountAsync(int month = 0)
    {
        return await _context.Users.CountAsync(a => (month == 0) ? true :
                        (a.CreatedOn.Month == month && a.CreatedOn.Year == DateTime.Now.Year));
    }
}
