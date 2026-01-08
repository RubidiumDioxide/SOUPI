using Microsoft.AspNetCore.Http; 


namespace SOUPICore.Services.Interfaces
{
    public interface IImageService
    {
        public Task<(byte[] Data, string ContentType)> GetByFileName(string fileName);

        public Task<string> Upload(IFormFile file);

        public Task Delete(string fileName);   
    }
}
