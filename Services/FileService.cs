using OloEcomm.Interface;
using System.IO.Compression;

namespace OloEcomm.Services
{
    public class FileService: IFileService
    {

     
        public async Task<string> SaveFileAsync(IFormFile imageFile, string[] allowedFileExtension)
        {
           if (imageFile == null)
            {
                throw new ArgumentNullException(nameof(imageFile));
            }

           


            var ext = Path.GetExtension(imageFile.FileName).ToLower();

            if (!allowedFileExtension.Contains(ext)) 
            {
                throw new ArgumentException($"Only {string.Join(",", allowedFileExtension)} are allowed");
            }

            using var memoryStream = new MemoryStream();
            await imageFile.CopyToAsync(memoryStream);
            byte[] imageBytes = memoryStream.ToArray();

            using (var outputStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(outputStream, CompressionMode.Compress))
                {
                    gzipStream.Write(imageBytes, 0, imageBytes.Length);
                }
                byte[] compressedBytes = outputStream.ToArray();
                string compressedBase64 = Convert.ToBase64String(compressedBytes);
            }
        }

       
    }
}
