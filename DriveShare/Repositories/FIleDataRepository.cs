using DriveShare.Data;
using DriveShare.Models;
using DriveShare.Models.Enums;
using DriveShare.Repositories.Interfaces;

namespace DriveShare.Repositories
{
    public class FIleDataRepository: IFileData
    {
        private readonly ApplicationDbContext _context;

        public FIleDataRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<FileData> GetFiles(string sortProperty, SortOrder sortOrder)
        {
            throw new NotImplementedException();
        }
    }
}
