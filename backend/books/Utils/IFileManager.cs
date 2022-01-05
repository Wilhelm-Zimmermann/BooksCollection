namespace backend.Utils
{
    public interface IFileManager
    {
        Task SaveImage(IFormFile image, string formatedFileName, string subPath);
        Task RemoveImage(string formatedFileName, string subPath);
    }
}
