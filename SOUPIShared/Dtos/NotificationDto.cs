using SOUPIShared.Models;

namespace SOUPIShared.Dtos;

public class NotificationDto
{
    public int Id { get; set; }

    public int RecieverId { get; set; }

    public int SenderId { get; set; }

    public string Text { get; set; } = null!;

    public string Link { get; set; } = null!;

    public NotificationDto(Notification notification)
    {
        Id = notification.Id; 
        RecieverId = notification.RecieverId;   
        SenderId = notification.SenderId; 
        Text = notification.Text; 
        Link = notification.Link; 
    }

    public NotificationDto() { }
}
