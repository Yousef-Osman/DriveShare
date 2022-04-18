using DriveShare.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DriveShare.Data;

public class ApplicationDbContext: IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
    { }

    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
        //base.OnModelCreating(modelBuilder);

        //seeding UserStatus table (allready in migration)
        //modelBuilder.Entity<UserStatus>() 
        //    .HasData(new UserStatus { Id = 1, Status = "Active" },
        //             new UserStatus { Id = 2, Status = "Inactive" },
        //             new UserStatus { Id = 3, Status = "Blocked" }
        //    );
    //}

    public DbSet<FileData> Files { get; set; }
    public DbSet<UserStatus> UserStatus { get; set; }
}
