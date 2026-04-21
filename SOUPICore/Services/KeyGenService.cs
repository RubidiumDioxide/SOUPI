using Microsoft.Extensions.Logging;
using SOUPICore.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;


namespace SOUPICore.Services
{
    public class KeyGenService : IKeyGenService 
    {
        private readonly ILogger<KeyGenService> _logger;
        private readonly string _masterKey; 

        public KeyGenService(ILogger<KeyGenService> logger, string masterKey)
        {
            _logger = logger;  
            _masterKey = masterKey; 
        }

        public string GenerateWebhookSecret(long repositoryId)
        {
            try
            {
                var keyBytes = Encoding.UTF8.GetBytes(_masterKey);
                var inputBytes = Encoding.UTF8.GetBytes(repositoryId.ToString());

                using (var hmac = new HMACSHA256(keyBytes))
                {
                    var hashBytes = hmac.ComputeHash(inputBytes);
                    // Convert to hex string to make it compatible with GitHub's secret field
                    return Convert.ToHexString(hashBytes).ToLower();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public bool VerifySignature(long repositoryId, string signature, string payload)
        {
            try
            {
                if (string.IsNullOrEmpty(signature)) return false;

                var expectedSecret = GenerateWebhookSecret(repositoryId);

                var keyBytes = Encoding.UTF8.GetBytes(expectedSecret);
                var bodyBytes = Encoding.UTF8.GetBytes(payload);

                // Initialize HMACSHA256 with secret key to hash the payload.
                using var hmac = new HMACSHA256(keyBytes);
                var hash = hmac.ComputeHash(bodyBytes);

                // GitHub prefixes the hash with "sha256=" and sends it in lowercase hex. 
                var expectedSignature = "sha256=" + Convert.ToHexString(hash).ToLower();

                return CryptographicOperations.FixedTimeEquals(
                    Encoding.UTF8.GetBytes(expectedSignature),
                    Encoding.UTF8.GetBytes(signature)
                ); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
    }
}
