using Microsoft.AspNetCore.Components.Forms;
using SOUPIShared.Exceptions;
using SOUPI.Handlers.Interfaces; 


namespace SOUPI.Handlers
{
    public class ImageRequestHandler : IImageRequestHandler 
    {
        private readonly ILogger<ImageRequestHandler> _logger;
        private readonly IWebHostEnvironment _environment; 

        public ImageRequestHandler(ILogger<ImageRequestHandler> logger, IWebHostEnvironment environment) 
        {
            _logger = logger;
            _environment = environment; 
        } 
        
        public async Task<string> Upload(IBrowserFile imageFile, string fileName)
        {
            try
            {
                string trustedFileName = Guid.NewGuid() + Path.GetExtension(fileName); 

                var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads");

                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }

                string filePath = Path.Combine(uploadsPath, trustedFileName);
                
                using var stream = File.Create(filePath);
                await imageFile.OpenReadStream().CopyToAsync(stream);

                return trustedFileName; 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Произошла ошибка при загрузке изображения. Проект не будет создан. {ex}"); 
                throw new SoupiException("Произошла ошибка при загрузке изображения. Проект не будет создан. Попробуйте позже или сообщите об ошибке в техподдержку "); 
            }
        } 

        public void Delete(string fileName)
        {
            try
            {
                var filePath = Path.Combine(_environment.WebRootPath, "uploads", fileName);

                if (!File.Exists(filePath))
                {
                    throw new SoupiException("Изображение не было найдено в директории "); 
                }

                File.Delete(filePath); 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Произошла ошибка при удалении изображения. {ex}");
            }
        }
    }
}
