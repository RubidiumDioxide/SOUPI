using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SOUPICore.Services.Interfaces;
using SOUPIShared.Dtos.SOUPIDtos;


namespace SOUPICore.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class NotificationController : ControllerBase
    {
        private readonly ILogger<NotificationController> _logger;
        private readonly INotificationService _notificationService;

        public NotificationController(ILogger<NotificationController> logger, INotificationService notificationService)
        {
            _logger = logger;
            _notificationService = notificationService;
        }

        [HttpGet("{receiverId}")]
        public async Task<ActionResult<IEnumerable<NotificationDisplayDto>>> GetByReceiverId([FromRoute] Guid receiverId)
        {
            try
            {
                var notifications = await _notificationService.GetByReceiverId(receiverId);

                return Ok(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500);
            }
        }

        [HttpPost]
        public async Task<ActionResult<NotificationDto>> Create([FromBody] NotificationDto newNotificationDto)
        {
            try
            {
                var notification = await _notificationService.Create(newNotificationDto);

                return Ok(notification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500);
            }
        }

        [HttpGet("{notificationId}")]
        public async Task<ActionResult<NotificationDto>> AcceptInvite([FromRoute] Guid notificationId)
        {
            try
            {
                var notification = await _notificationService.AcceptInvite(notificationId);

                return Ok(notification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500);
            }
        }

        [HttpGet("{notificationId}")]
        public async Task<ActionResult<NotificationDto>> MarkAsViewed([FromRoute] Guid notificationId)
        {
            try
            {
                var notification = await _notificationService.MarkAsViewed(notificationId);

                return Ok(notification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500);
            }
        }
    }
}