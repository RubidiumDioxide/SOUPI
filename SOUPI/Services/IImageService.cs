using Microsoft.AspNetCore.Components.Forms;


namespace SOUPI.Services
{
    public interface IImageService
    {
        public Task<string> Upload(IBrowserFile imageFile, string fileName);

        public void Delete(string fileName); 
    }
}
