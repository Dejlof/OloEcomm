using OloEcomm.Interface;

namespace OloEcomm.Services
{
    public class FileService: IFileService
    {

        private readonly string _imageDirectory;

        public FileService(IConfiguration configuration)
        {
            _imageDirectory = configuration["ImageUpload:Directory"];
        }
        public async Task<string> SaveFileAsync(IFormFile imageFile, string[] allowedFileExtension)
        {
           if (imageFile == null)
            {
                throw new ArgumentNullException(nameof(imageFile));
            }

           

            if (!Directory.Exists(_imageDirectory)) 
            { 
             Directory.CreateDirectory(_imageDirectory);
            }

            var ext = Path.GetExtension(imageFile.FileName);

            if (!allowedFileExtension.Contains(ext)) 
            {
                throw new ArgumentException($"Only {string.Join(",", allowedFileExtension)} are allowed");
            }

            var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(_imageDirectory, fileName);
            using var stream = new FileStream(filePath, FileMode.Create);
            await imageFile.CopyToAsync(stream);

            var imageUrl = Path.Combine("/images", fileName); 
            return imageUrl;
        }

        public async Task<bool> DeleteFileAsync(string fileNameWithExtension)
        {
            if (string.IsNullOrEmpty(fileNameWithExtension))
            {
                throw new ArgumentNullException(nameof(fileNameWithExtension));
            }

            var filePath = Path.Combine(_imageDirectory, Path.GetFileName(fileNameWithExtension));

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return await Task.FromResult(true);
            }

            return await Task.FromResult(false);
        }
    }
}
