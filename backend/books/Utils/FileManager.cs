namespace backend.Utils
{
    public class FileManager : IFileManager
    {
        private readonly IWebHostEnvironment _environment;

        public FileManager(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task SaveImage(IFormFile image, string formatedFileName, string subPath)
        {
            var filePath = Path.Combine(_environment.ContentRootPath, "Images", subPath, formatedFileName);

            using (var fs = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(fs);
            }
        }

        public async Task RemoveImage(string formatedFileName, string subPath)
        {
            var filePath = Path.Combine(_environment.ContentRootPath, "Images",subPath, formatedFileName);

            File.Delete(filePath);
        }
    }
}
