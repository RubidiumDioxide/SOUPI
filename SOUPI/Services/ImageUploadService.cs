using Microsoft.AspNetCore.Components.Forms;
using SOUPIShared.Exceptions; 


namespace SOUPI.Services
{
    public class ImageUploadService : IImageUploadService
    {
        private readonly ILogger<ImageUploadService> _logger;
        private readonly IWebHostEnvironment _environment; 

        public ImageUploadService(ILogger<ImageUploadService> logger, IWebHostEnvironment environment) 
        {
            _logger = logger;
            _environment = environment; 
        } 
        
        public async Task<string> UploadImage(IBrowserFile imageFile)
        {
            try
            {
                var trustedFileName = Path.GetRandomFileName(); 

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
    }
}
