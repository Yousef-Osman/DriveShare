using AutoMapper;
using DriveShare.Areas.Admin.ViewModels;
using DriveShare.Data;
using DriveShare.Models;
using DriveShare.ViewModels;

namespace DriveShare.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<FileData, FileDataViewModel>().ReverseMap(); //not used yet
        CreateMap<UploadFileViewModel, FileData>(); //not used yet

        CreateMap<ApplicationUser, UserViewModel>(); //not used yet
    }
}