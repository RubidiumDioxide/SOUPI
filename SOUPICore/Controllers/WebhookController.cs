using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SOUPICore.Misc;
using SOUPICore.Services.Interfaces;
using SOUPIShared.Dtos.SOUPIDtos;
using SOUPIShared.Exceptions;
using SOUPIShared.Extensions;
using System.Text.Json;
using System.Text.RegularExpressions;


namespace SOUPICore.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]/[action]")]
    public class WebhookController : ControllerBase 
    {
        private readonly ILogger<WebhookController> _logger;
        private readonly IKeyGenService _keyGenService;
        private readonly IActivityService _activityService; 

        public WebhookController(ILogger<WebhookController> logger, IKeyGenService keyGenService, IActivityService activityService)
        {
            _logger = logger; 
            _keyGenService = keyGenService;  
            _activityService = activityService; 
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

                if (!payloadObj.Repository.Name.IsValidGitHubRepositoryName())
                {
                    return BadRequest();
                }

                if (payloadObj.Ref != null && payloadObj.Commits != null && payloadObj.Commits.Count != 0)
                {
                    // regex to capture jobs within the commit message 
                    var jobRegex = new Regex(@"\[([a-zA-Z0-9\u0400-\u04FF\s]+)\]", RegexOptions.Compiled);

                    ILookup<string, GitHubPushPayload.CommitInfo> jobCommits = payloadObj.Commits
                        .SelectMany(commit => jobRegex.Matches(commit.Message)
                            .Select(m => m.Groups[1].Value.Trim())
                            .Where(name => !string.IsNullOrWhiteSpace(name))
                            .Distinct(StringComparer.OrdinalIgnoreCase) // remove duplicates within a commit
                            .Select(jobName => new { jobName, commit }))
                        .ToLookup(
                            x => x.jobName,
                            x => x.commit,
                            StringComparer.OrdinalIgnoreCase
                        );

                    // create activities 
                    if (jobCommits.Any())
                    {
                        await _activityService.CreateSet(jobCommits);
                    }
                }

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
