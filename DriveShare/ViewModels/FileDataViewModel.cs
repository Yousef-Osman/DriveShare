﻿using System.ComponentModel.DataAnnotations;

namespace DriveShare.ViewModels;

public class FileDataViewModel
{
    public string Id { get; set; }
    [Display(Name ="File Name")]
    public string FileName { get; set; }
    public string FileSerial { get; set; }
    [Display(Name = "File Type")]
    public string ContentType { get; set; }
    public decimal Size { get; set; }
    [Display(Name = "Download Count")]
    public int DownloadCount { get; set; }
    [Display(Name = "Date Created")]
    public string CreatedOn { get; set; }
    [Display(Name = "Last Modified")]
    public string LastModifiedOn { get; set; }
}
