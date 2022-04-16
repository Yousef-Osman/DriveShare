using AutoMapper;
using DriveShare.Models;
using DriveShare.ViewModels;

namespace DriveShare.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<FileData, FileDataViewModel>().ReverseMap(); //not used yet
        CreateMap<UploadFileViewModel, FileData>(); //not used yet
    }
}