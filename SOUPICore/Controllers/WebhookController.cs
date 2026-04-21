using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SOUPICore.Services.Interfaces;
using SOUPIShared.Dtos;
using SOUPIShared.Exceptions;
using System.Text.Json; 


namespace SOUPICore.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]/[action]")]
    public class WebhookController : ControllerBase 
    {
        private readonly ILogger<WebhookController> _logger;
        private readonly IKeyGenService _keyGenService; 

        public WebhookController(ILogger<WebhookController> logger, IKeyGenService keyGenService)
        {
            _logger = logger; 
            _keyGenService = keyGenService;  
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<ActivityDto>>> Push()
        {
            try
            {
                using var reader = new StreamReader(Request.Body);
                var rawPayload = await reader.ReadToEndAsync();

                var signature = Request.Headers["X-Hub-Signature-256"].ToString();
                var payloadObj = JsonSerializer.Deserialize<GitHubPushPayload>(rawPayload,
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

                if (payloadObj == null)
                {
                    throw new BadRequestException();
                }

                if (!_keyGenService.VerifySignature(payloadObj.Repository.Id, signature, rawPayload))
                {
                    return Unauthorized("Ошибка проверки подписи ");
                }

                // logic
                
                return Ok();
            }
            catch (BadRequestException)
            {
                return BadRequest(); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500);
            }
        }
    }
}
