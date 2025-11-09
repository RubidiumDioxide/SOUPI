using Microsoft.AspNetCore.Components.Forms;


namespace SOUPI.Services
{
    public interface IImageUploadService
    {
        public Task<string> UploadImage(IBrowserFile imageFile); 
    }
}
