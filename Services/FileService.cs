using OloEcomm.Interface;

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

            string base64String = Convert.ToBase64String(imageBytes);
            return base64String;
        }

       
    }
}
