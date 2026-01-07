using Microsoft.AspNetCore.Components.Forms;


namespace SOUPI.Handlers.Interfaces
{
    public interface IImageRequestHandler
    {
        public Task<string> Upload(IBrowserFile imageFile, string fileName);
        public void Delete(string fileName); 
    }
}
