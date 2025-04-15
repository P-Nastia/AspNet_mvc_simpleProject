namespace Web1.Interfaces;

public interface IImageService
{
    Task<string> SaveImageAsync(IFormFile file);
}
