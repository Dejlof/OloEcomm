using System.Threading.Tasks;

namespace OloEcomm.Interface
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile imageFile, string[] allowedFileExtension);
        Task<bool> DeleteFileAsync(string fileNameWithExtension);
    }
}
