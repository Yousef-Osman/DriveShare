using DriveShare.Areas.Admin.Repositories;
using DriveShare.Areas.Admin.Repositories.Interfaces;

namespace DriveShare.Areas.Admin;

public static class AdminStartUp
{
    public static IServiceCollection AddAdminServices(this IServiceCollection services)
    {
        services.AddScoped<IUser, UserRepository>();

        return services;
    }

}
