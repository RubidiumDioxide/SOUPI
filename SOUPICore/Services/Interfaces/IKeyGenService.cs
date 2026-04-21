namespace SOUPICore.Services.Interfaces
{
    public interface IKeyGenService
    {
        public string GenerateWebhookSecret(long repositoryId);

        public bool VerifySignature(long repositoryId, string signature, string payload); 
    }
}
